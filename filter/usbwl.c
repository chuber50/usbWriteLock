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


//NTSTATUS
//SpyMessage (
//    _In_ PVOID ConnectionCookie,
//    _In_reads_bytes_opt_(InputBufferSize) PVOID InputBuffer,
//    _In_ ULONG InputBufferSize,
//    _Out_writes_bytes_to_opt_(OutputBufferSize,*ReturnOutputBufferLength) PVOID OutputBuffer,
//    _In_ ULONG OutputBufferSize,
//    _Out_ PULONG ReturnOutputBufferLength
//    );

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

//NTSTATUS
//SpyEnlistInTransaction (
//    _In_ PCFLT_RELATED_OBJECTS FltObjects
//    );

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

    This routine receives ALL pre-operation callbacks for this filter.  It then
    tries to log information about the given operation.  If we are able
    to log information then we will call our post-operation callback  routine.

    NOTE:  This routine must be NON-PAGED because it can be called on the
           paging path.

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
		if (Data->Iopb->MajorFunction == IRP_MJ_CREATE)
		{
			//http://www.osronline.com/showThread.cfm?link=79773
			PFLT_IO_PARAMETER_BLOCK IopbPtr = Data->Iopb;
			PFLT_PARAMETERS ParameterPtr = &IopbPtr->Parameters;
			PIO_SECURITY_CONTEXT SecurityContextPtr = ParameterPtr->Create.SecurityContext;

			ACCESS_MASK desiredAccess = SecurityContextPtr->DesiredAccess;
			ACCESS_MASK createDisposition = ParameterPtr->Create.Options;

			//SecurityContextPtr->AccessState.

			const BOOLEAN writeOperation = ((desiredAccess & 
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

			const BOOLEAN overwriteOperation = (createDisposition & 
				(FILE_SUPERSEDE | 
					FILE_CREATE | 
					FILE_OVERWRITE | 
					FILE_OVERWRITE_IF | 
					FILE_ADD_FILE | 
					FILE_ADD_SUBDIRECTORY));

			if (writeOperation || overwriteOperation) {
				Data->IoStatus.Status = STATUS_ACCESS_DENIED; //STATUS_CANCELLED; 
				Data->IoStatus.Information = 0;
				DbgPrint("IRP Major: %u \n", Data->Iopb->MajorFunction);
				DbgPrint("IRP Minor: %u \n", Data->Iopb->MinorFunction);
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

    This routine receives ALL post-operation callbacks.  This will take
    the log record passed in the context parameter and update it with
    the completion information.  It will then insert it on a list to be
    sent to the usermode component.

    NOTE:  This routine must be NON-PAGED because it can be called at DPC level

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

    //
    //  If our instance is in the process of being torn down don't bother to
    //  log this record, free it now.
    //

	(void)Flags;
	(void)CompletionContext;
	(void)Data;
	(void)FltObjects;


    return FLT_POSTOP_FINISHED_PROCESSING;
}

//
//NTSTATUS
//SpyEnlistInTransaction (
//    _In_ PCFLT_RELATED_OBJECTS FltObjects
//    )
///*++
//
//Routine Description
//
//    Minispy calls this function to enlist in a transaction of interest. 
//
//Arguments
//
//    FltObjects - Contains parameters required to enlist in a transaction.
//
//Return value
//
//    Returns STATUS_SUCCESS if we were able to successfully enlist in a new transcation or if we
//    were already enlisted in the transaction. Returns an appropriate error code on a failure.
//    
//--*/
//{
//
//#if USBWL_VISTA
//
//    PMINISPY_TRANSACTION_CONTEXT transactionContext = NULL;
//    PMINISPY_TRANSACTION_CONTEXT oldTransactionContext = NULL;
//    PRECORD_LIST recordList;
//    NTSTATUS status;
//    static ULONG Sequence=1;
//
//    //
//    //  This code is only built in the Vista environment, but
//    //  we need to ensure this binary still runs down-level.  Return
//    //  at this point if the transaction dynamic imports were not found.
//    //
//    //  If we find FltGetTransactionContext, we assume the other
//    //  transaction APIs are also present.
//    //
//
//    if (NULL == UsbWlData.PFltGetTransactionContext) {
//
//        return STATUS_SUCCESS;
//    }
//
//    //
//    //  Try to get our context for this transaction. If we get
//    //  one we have already enlisted in this transaction.
//    //
//
//    status = (*UsbWlData.PFltGetTransactionContext)( FltObjects->Instance,
//                                                       FltObjects->Transaction,
//                                                       &transactionContext );
//
//    if (NT_SUCCESS( status )) {
//
//        // 
//        //  Check if we have already enlisted in the transaction. 
//        //
//
//        if (FlagOn(transactionContext->Flags, MINISPY_ENLISTED_IN_TRANSACTION)) {
//
//            //
//            //  FltGetTransactionContext puts a reference on the context. Release
//            //  that now and return success.
//            //
//            
//            FltReleaseContext( transactionContext );
//            return STATUS_SUCCESS;
//        }
//
//        //
//        //  If we have not enlisted then we need to try and enlist in the transaction.
//        //
//        
//        goto ENLIST_IN_TRANSACTION;
//    }
//
//    //
//    //  If the context does not exist create a new one, else return the error
//    //  status to the caller.
//    //
//
//    if (status != STATUS_NOT_FOUND) {
//
//        return status;
//    }
//
//    //
//    //  Allocate a transaction context.
//    //
//
//    status = FltAllocateContext( FltObjects->Filter,
//                                 FLT_TRANSACTION_CONTEXT,
//                                 sizeof(USBWL_TRANSACTION_CONTEXT),
//                                 PagedPool,
//                                 &transactionContext );
//
//    if (!NT_SUCCESS( status )) {
//
//        return status;
//    }
//
//    //
//    //  Set the context into the transaction
//    //
//
//    RtlZeroMemory(transactionContext, sizeof(USBWL_TRANSACTION_CONTEXT));
//    transactionContext->Count = Sequence++;
//
//    FLT_ASSERT( UsbWlData.PFltSetTransactionContext );
//
//    status = (*UsbWlData.PFltSetTransactionContext)( FltObjects->Instance,
//                                                       FltObjects->Transaction,
//                                                       FLT_SET_CONTEXT_KEEP_IF_EXISTS,
//                                                       transactionContext,
//                                                       &oldTransactionContext );
//
//    if (!NT_SUCCESS( status )) {
//
//        FltReleaseContext( transactionContext );    //this will free the context
//
//        if (status != STATUS_FLT_CONTEXT_ALREADY_DEFINED) {
//
//            return status;
//        }
//
//        FLT_ASSERT(oldTransactionContext != NULL);
//        
//        if (FlagOn(oldTransactionContext->Flags, MINISPY_ENLISTED_IN_TRANSACTION)) {
//
//            //
//            //  If this context is already enlisted then release the reference
//            //  which FltSetTransactionContext put on it and return success.
//            //
//            
//            FltReleaseContext( oldTransactionContext );
//            return STATUS_SUCCESS;
//        }
//
//        //
//        //  If we found an existing transaction then we should try and
//        //  enlist in it. There is a race here in which the thread 
//        //  which actually set the transaction context may fail to 
//        //  enlist in the transaction and delete it later. It might so
//        //  happen that we picked up a reference to that context here
//        //  and successfully enlisted in that transaction. For now
//        //  we have chosen to ignore this scenario.
//        //
//
//        //
//        //  If we are not enlisted then assign the right transactionContext
//        //  and attempt enlistment.
//        //
//
//        transactionContext = oldTransactionContext;            
//    }
//
//ENLIST_IN_TRANSACTION: 
//
//    //
//    //  Enlist on this transaction for notifications.
//    //
//
//    FLT_ASSERT( UsbWlData.PFltEnlistInTransaction );
//
//    status = (*UsbWlData.PFltEnlistInTransaction)( FltObjects->Instance,
//                                                     FltObjects->Transaction,
//                                                     transactionContext,
//                                                     FLT_MAX_TRANSACTION_NOTIFICATIONS );
//
//    //
//    //  If the enlistment failed we might have to delete the context and remove
//    //  our count.
//    //
//
//    if (!NT_SUCCESS( status )) {
//
//        //
//        //  If the error is that we are already enlisted then we do not need
//        //  to delete the context. Otherwise we have to delete the context
//        //  before releasing our reference.
//        //
//        
//        if (status == STATUS_FLT_ALREADY_ENLISTED) {
//
//            status = STATUS_SUCCESS;
//
//        } else {
//
//            //
//            //  It is worth noting that only the first caller of
//            //  FltDeleteContext will remove the reference added by
//            //  filter manager when the context was set.
//            //
//            
//            FltDeleteContext( transactionContext );
//        }
//        
//        FltReleaseContext( transactionContext );
//        return status;
//    }
//
//    //
//    //  Set the flag so that future enlistment efforts know that we
//    //  successfully enlisted in the transaction.
//    //
//
//    SetFlagInterlocked( &transactionContext->Flags, MINISPY_ENLISTED_IN_TRANSACTION );
//    
//    //
//    //  The operation succeeded, remove our count
//    //
//
//    FltReleaseContext( transactionContext );
//
//    //
//    //  Log a record that a new transaction has started.
//    //
//
//    recordList = SpyNewRecord();
//
//    if (recordList) {
//
//        SpyLogTransactionNotify( FltObjects, recordList, 0 );
//
//        //
//        //  Send the logged information to the user service.
//        //
//
//        SpyLog( recordList );
//    }
//
//#endif // USBWL_VISTA
//
//    return STATUS_SUCCESS;
//}


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


//LONG
//SpyExceptionFilter (
//    _In_ PEXCEPTION_POINTERS ExceptionPointer,
//    _In_ BOOLEAN AccessingUserBuffer
//    )
///*++
//
//Routine Description:
//
//    Exception filter to catch errors touching user buffers.
//
//Arguments:
//
//    ExceptionPointer - The exception record.
//
//    AccessingUserBuffer - If TRUE, overrides FsRtlIsNtStatusExpected to allow
//                          the caller to munge the error to a desired status.
//
//Return Value:
//
//    EXCEPTION_EXECUTE_HANDLER - If the exception handler should be run.
//
//    EXCEPTION_CONTINUE_SEARCH - If a higher exception handler should take care of
//                                this exception.
//
//--*/
//{
//    NTSTATUS Status;
//
//    Status = ExceptionPointer->ExceptionRecord->ExceptionCode;
//
//    //
//    //  Certain exceptions shouldn't be dismissed within the namechanger filter
//    //  unless we're touching user memory.
//    //
//
//    if (!FsRtlIsNtstatusExpected( Status ) &&
//        !AccessingUserBuffer) {
//
//        return EXCEPTION_CONTINUE_SEARCH;
//    }
//
//    return EXCEPTION_EXECUTE_HANDLER;
//}


