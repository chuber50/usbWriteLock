/*++

Copyright (c) 1989-2002  Microsoft Corporation

Environment:

    Kernel mode

--*/

#include "usbWlKern.h"
#include <stdio.h>

//
//  Global variables
//

USBWL_DATA UsbWlData;
NTSTATUS StatusToBreakOn = 0;

//---------------------------------------------------------------------------
//  Function prototypes
//---------------------------------------------------------------------------
DRIVER_INITIALIZE DriverEntry;
NTSTATUS
DriverEntry (
    _In_ PDRIVER_OBJECT DriverObject,
    _In_ PUNICODE_STRING RegistryPath
    );

NTSTATUS
WlCommunicationConnect(
    _In_ PFLT_PORT ClientPort,
    _In_ PVOID ServerPortCookie,
    _In_reads_bytes_(SizeOfContext) PVOID ConnectionContext,
    _In_ ULONG SizeOfContext,
    _Flt_ConnectionCookie_Outptr_ PVOID *ConnectionCookie
    );

VOID
WlCommunicationDisconnect(
    _In_opt_ PVOID ConnectionCookie
    );

//---------------------------------------------------------------------------
//  Assign text sections for each routine.
//---------------------------------------------------------------------------

#ifdef ALLOC_PRAGMA
    #pragma alloc_text(INIT, DriverEntry)
    #pragma alloc_text(PAGE, WlFilterUnload)
    #pragma alloc_text(PAGE, WlQueryTeardown)
    #pragma alloc_text(PAGE, WlCommunicationConnect)
    #pragma alloc_text(PAGE, WlCommunicationDisconnect)
#endif


#define SetFlagInterlocked(_ptrFlags,_flagToSet) \
    ((VOID)InterlockedOr(((volatile LONG *)(_ptrFlags)),_flagToSet))
    
//---------------------------------------------------------------------------
//                      ROUTINES
//---------------------------------------------------------------------------

NTSTATUS
DriverEntry (
    _In_ PDRIVER_OBJECT DriverObject,
    _In_ PUNICODE_STRING RegistryPath
    )
/*++

Routine Description:

    This routine is called when a driver first loads.  Its purpose is to
    initialize global state and then register with FltMgr to start filtering.

Arguments:

    DriverObject - Pointer to driver object created by the system to
        represent this driver.
    RegistryPath - Unicode string identifying where the parameters for this
        driver are located in the registry.

Return Value:

    Status of the operation.

--*/
{
    PSECURITY_DESCRIPTOR sd;
    OBJECT_ATTRIBUTES oa;
    UNICODE_STRING uniString;
    NTSTATUS status = STATUS_SUCCESS;

	UNREFERENCED_PARAMETER(RegistryPath);

    try {

        //
        // Initialize global data structures.
        //

        UsbWlData.LogSequenceNumber = 0;
        UsbWlData.MaxRecordsToAllocate = DEFAULT_MAX_RECORDS_TO_ALLOCATE;
        UsbWlData.RecordsAllocated = 0;
        UsbWlData.NameQueryMethod = DEFAULT_NAME_QUERY_METHOD;

        UsbWlData.DriverObject = DriverObject;

        InitializeListHead( &UsbWlData.OutputBufferList );
        KeInitializeSpinLock( &UsbWlData.OutputBufferLock );

        ExInitializeNPagedLookasideList( &UsbWlData.FreeBufferList,
                                         NULL,
                                         NULL,
                                         POOL_NX_ALLOCATION,
                                         RECORD_SIZE,
                                         WL_TAG,
                                         0 );

#if USBWL_VISTA

        //
        //  Dynamically import FilterMgr APIs for transaction support
        //

#pragma warning(push)
#pragma warning(disable:4055) // type cast from data pointer to function pointer
        UsbWlData.PFltSetTransactionContext = (PFLT_SET_TRANSACTION_CONTEXT) FltGetRoutineAddress( "FltSetTransactionContext" );
        UsbWlData.PFltGetTransactionContext = (PFLT_GET_TRANSACTION_CONTEXT) FltGetRoutineAddress( "FltGetTransactionContext" );
        UsbWlData.PFltEnlistInTransaction = (PFLT_ENLIST_IN_TRANSACTION) FltGetRoutineAddress( "FltEnlistInTransaction" );
#pragma warning(pop)

#endif


        //
        //  Now that our global configuration is complete, register with FltMgr.
        //

        status = FltRegisterFilter( DriverObject,
                                    &FilterRegistration,
                                    &UsbWlData.Filter );

        if (!NT_SUCCESS( status )) {

           leave;
        }


        status  = FltBuildDefaultSecurityDescriptor( &sd,
                                                     FLT_PORT_ALL_ACCESS );

        if (!NT_SUCCESS( status )) {
            leave;
        }

        RtlInitUnicodeString( &uniString, WRITELOCK_PORT_NAME );

        InitializeObjectAttributes( &oa,
                                    &uniString,
                                    OBJ_KERNEL_HANDLE | OBJ_CASE_INSENSITIVE,
                                    NULL,
                                    sd );

        status = FltCreateCommunicationPort( UsbWlData.Filter,
                                             &UsbWlData.ServerPort,
                                             &oa,
                                             NULL,
                                             WlCommunicationConnect,
                                             WlCommunicationDisconnect,
                                             NULL,
                                             1 );

        FltFreeSecurityDescriptor( sd );

        if (!NT_SUCCESS( status )) {
            leave;
        }

        //
        //  We are now ready to start filtering
        //

        status = FltStartFiltering( UsbWlData.Filter );

    } finally {

        if (!NT_SUCCESS( status ) ) {

             if (NULL != UsbWlData.ServerPort) {
                 FltCloseCommunicationPort( UsbWlData.ServerPort );
             }

             if (NULL != UsbWlData.Filter) {
                 FltUnregisterFilter( UsbWlData.Filter );
             }

             ExDeleteNPagedLookasideList( &UsbWlData.FreeBufferList );
        }
    }

    return status;
}

NTSTATUS
WlCommunicationConnect(
    _In_ PFLT_PORT ClientPort,
    _In_ PVOID ServerPortCookie,
    _In_reads_bytes_(SizeOfContext) PVOID ConnectionContext,
    _In_ ULONG SizeOfContext,
    _Flt_ConnectionCookie_Outptr_ PVOID *ConnectionCookie
    )
/*++

Routine Description

    This is called when user-mode connects to the server
    port - to establish a connection

Arguments

    ClientPort - This is the pointer to the client port that
        will be used to send messages from the filter.
    ServerPortCookie - unused
    ConnectionContext - unused
    SizeofContext   - unused
    ConnectionCookie - unused

Return Value

    STATUS_SUCCESS - to accept the connection
--*/
{

    PAGED_CODE();

    UNREFERENCED_PARAMETER( ServerPortCookie );
    UNREFERENCED_PARAMETER( ConnectionContext );
    UNREFERENCED_PARAMETER( SizeOfContext);
    UNREFERENCED_PARAMETER( ConnectionCookie );

    FLT_ASSERT( UsbWlData.ClientPort == NULL );
    UsbWlData.ClientPort = ClientPort;
    return STATUS_SUCCESS;
}


VOID
WlCommunicationDisconnect(
    _In_opt_ PVOID ConnectionCookie
   )
/*++

Routine Description

    This is called when the connection is torn-down. We use it to close our handle to the connection

Arguments

    ConnectionCookie - unused

Return value

    None
--*/
{

    PAGED_CODE();

    UNREFERENCED_PARAMETER( ConnectionCookie );

    //
    //  Close our handle
    //

    FltCloseClientPort( UsbWlData.Filter, &UsbWlData.ClientPort );
}

NTSTATUS
WlFilterUnload (
    _In_ FLT_FILTER_UNLOAD_FLAGS Flags
    )
/*++

Routine Description:

    This is called when a request has been made to unload the filter.  Unload
    requests from the Operation System (ex: "sc stop minispy" can not be
    failed.  Other unload requests may be failed.

    You can disallow OS unload request by setting the
    FLTREGFL_DO_NOT_SUPPORT_SERVICE_STOP flag in the FLT_REGISTARTION
    structure.

Arguments:

    Flags - Flags pertinent to this operation

Return Value:

    Always success

--*/
{
    UNREFERENCED_PARAMETER( Flags );

    PAGED_CODE();

    //
    //  Close the server port. This will stop new connections.
    //

    FltCloseCommunicationPort( UsbWlData.ServerPort );

    FltUnregisterFilter( UsbWlData.Filter );

    ExDeleteNPagedLookasideList( &UsbWlData.FreeBufferList );

    return STATUS_SUCCESS;
}


NTSTATUS
WlQueryTeardown (
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _In_ FLT_INSTANCE_QUERY_TEARDOWN_FLAGS Flags
    )
/*++

Routine Description:

    This allows our filter to be manually detached from a volume.

Arguments:

    FltObjects - Contains pointer to relevant objects for this operation.
        Note that the FileObject field will always be NULL.

    Flags - Flags pertinent to this operation

Return Value:

--*/
{
    UNREFERENCED_PARAMETER( FltObjects );
    UNREFERENCED_PARAMETER( Flags );
    PAGED_CODE();
    return STATUS_SUCCESS;
}




//---------------------------------------------------------------------------
//              Operation filtering routines
//---------------------------------------------------------------------------


FLT_PREOP_CALLBACK_STATUS
#pragma warning(suppress: 6262) // higher than usual stack usage is considered safe in this case
WlPreOperationCallback (
    _Inout_ PFLT_CALLBACK_DATA Data,
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _Flt_CompletionContext_Outptr_ PVOID *CompletionContext
    )
/*++

Routine Description:

    This routine receives ALL pre-operation callbacks for this filter. It then
	tries to block the write-specific ones and suppresses the post-operation callback.

Arguments:

    Data - Contains information about the given operation.

    FltObjects - Contains pointers to the various objects that are pertinent
        to this operation.

    CompletionContext - This receives the address of our log buffer for this
        operation.  Our completion routine then receives this buffer address.

Return Value:

    Identifies how processing should continue for this operation

--*/
{
    FLT_PREOP_CALLBACK_STATUS returnStatus = FLT_PREOP_SUCCESS_NO_CALLBACK; //assume we are NOT going to call our completion routine

	if (Data && Data->Iopb)
	{
		// we just look at IRP_MJ_CREATE
		if (Data->Iopb->MajorFunction == IRP_MJ_CREATE)
		{
			// extract the relevant bitmasks
			PFLT_IO_PARAMETER_BLOCK IopbPtr = Data->Iopb;
			PFLT_PARAMETERS ParameterPtr = &IopbPtr->Parameters;
			PIO_SECURITY_CONTEXT SecurityContextPtr = ParameterPtr->Create.SecurityContext;

			// these are not null if Iopb is not
			ACCESS_MASK desiredAccess = SecurityContextPtr->DesiredAccess;
			ACCESS_MASK createDisposition = ParameterPtr->Create.Options;

			// bitmask operation: detect write operation
			BOOLEAN writeOperation = ((desiredAccess & 
				(FILE_WRITE_DATA | 
					FILE_WRITE_ATTRIBUTES | 
					FILE_WRITE_EA | 
					FILE_APPEND_DATA | 
					DELETE | 
					WRITE_DAC | 
					WRITE_OWNER | 
					FILE_ADD_FILE | 
					FILE_ADD_SUBDIRECTORY)) ||
				(Data->Iopb->Parameters.Create.Options & FILE_DELETE_ON_CLOSE));

			// bitmask operation: detect overwrite operation
			BOOLEAN overwriteOperation = (createDisposition & 
				(FILE_SUPERSEDE | 
					FILE_CREATE | 
					FILE_OVERWRITE | 
					FILE_OVERWRITE_IF | 
					FILE_ADD_FILE | 
					FILE_ADD_SUBDIRECTORY));

			if (writeOperation || overwriteOperation) {

				// strip down remaining access bitmasks for STATUS_SUCCESS test
				//SecurityContextPtr->AccessState->PreviouslyGrantedAccess = 0;
				//SecurityContextPtr->AccessState->RemainingDesiredAccess = 0;

				Data->IoStatus.Status = STATUS_ACCESS_DENIED; //STATUS_CANCELLED; // STATUS_ACCESS_DENIED
			    
			    // suppress remaining information
				Data->IoStatus.Information = 0; 

				DbgPrint("IRP Major: %u \n", Data->Iopb->MajorFunction);
				DbgPrint("IRP Minor: %u \n", Data->Iopb->MinorFunction);

				// does not require Post-operation callback
				return FLT_PREOP_COMPLETE; 
			}
		}	
	}

	CompletionContext = NULL; // we don't pass data
	(void)FltObjects;

    return returnStatus;
}


FLT_POSTOP_CALLBACK_STATUS
WlPostOperationCallback (
    _Inout_ PFLT_CALLBACK_DATA Data,
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _In_ PVOID CompletionContext,
    _In_ FLT_POST_OPERATION_FLAGS Flags
    )
/*++

Routine Description:

    This routine has been stripped down to just pass the data to the next filter.

Arguments:

    Data - Contains information about the given operation.

    FltObjects - Contains pointers to the various objects that are pertinent
        to this operation.

    CompletionContext - Pointer to the RECORD_LIST structure in which we
        store the information we are logging.  This was passed from the
        pre-operation callback

    Flags - Contains information as to why this routine was called.

Return Value:

    Identifies how processing should continue for this operation

--*/
{
    UNREFERENCED_PARAMETER( FltObjects );

	(void)Flags;
	(void)CompletionContext;
	(void)Data;
	(void)FltObjects;


    return FLT_POSTOP_FINISHED_PROCESSING;
}

#if USBWL_VISTA

NTSTATUS
WlKtmNotificationCallback (
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _In_ PFLT_CONTEXT TransactionContext,
    _In_ ULONG TransactionNotification
    )
{
    UNREFERENCED_PARAMETER( TransactionContext );

	(void)FltObjects;
	(void)TransactionNotification;

    return STATUS_SUCCESS;
}

#endif // USBWL_VISTA

VOID
WlDeleteTxfContext (
    _Inout_ PMINISPY_TRANSACTION_CONTEXT Context,
    _In_ FLT_CONTEXT_TYPE ContextType
    )
{
    UNREFERENCED_PARAMETER( Context );
    UNREFERENCED_PARAMETER( ContextType );

    FLT_ASSERT(FLT_TRANSACTION_CONTEXT == ContextType);
    FLT_ASSERT(Context->Count != 0);
}