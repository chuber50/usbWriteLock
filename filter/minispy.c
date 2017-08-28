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
SpyMessage (
    _In_ PVOID ConnectionCookie,
    _In_reads_bytes_opt_(InputBufferSize) PVOID InputBuffer,
    _In_ ULONG InputBufferSize,
    _Out_writes_bytes_to_opt_(OutputBufferSize,*ReturnOutputBufferLength) PVOID OutputBuffer,
    _In_ ULONG OutputBufferSize,
    _Out_ PULONG ReturnOutputBufferLength
    );

NTSTATUS
SpyConnect(
    _In_ PFLT_PORT ClientPort,
    _In_ PVOID ServerPortCookie,
    _In_reads_bytes_(SizeOfContext) PVOID ConnectionContext,
    _In_ ULONG SizeOfContext,
    _Flt_ConnectionCookie_Outptr_ PVOID *ConnectionCookie
    );

VOID
SpyDisconnect(
    _In_opt_ PVOID ConnectionCookie
    );

NTSTATUS
SpyEnlistInTransaction (
    _In_ PCFLT_RELATED_OBJECTS FltObjects
    );

//---------------------------------------------------------------------------
//  Assign text sections for each routine.
//---------------------------------------------------------------------------

#ifdef ALLOC_PRAGMA
    #pragma alloc_text(INIT, DriverEntry)
    #pragma alloc_text(PAGE, WlFilterUnload)
    #pragma alloc_text(PAGE, WlQueryTeardown)
    #pragma alloc_text(PAGE, SpyConnect)
    #pragma alloc_text(PAGE, SpyDisconnect)
    #pragma alloc_text(PAGE, SpyMessage)
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
                                         SPY_TAG,
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
        // Read the custom parameters for MiniSpy from the registry
        //

        SpyReadDriverParameters(RegistryPath);

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
                                             SpyConnect,
                                             SpyDisconnect,
                                             SpyMessage,
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
SpyConnect(
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
SpyDisconnect(
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

    SpyEmptyOutputBufferList();
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


NTSTATUS
SpyMessage (
    _In_ PVOID ConnectionCookie,
    _In_reads_bytes_opt_(InputBufferSize) PVOID InputBuffer,
    _In_ ULONG InputBufferSize,
    _Out_writes_bytes_to_opt_(OutputBufferSize,*ReturnOutputBufferLength) PVOID OutputBuffer,
    _In_ ULONG OutputBufferSize,
    _Out_ PULONG ReturnOutputBufferLength
    )
/*++

Routine Description:

    This is called whenever a user mode application wishes to communicate
    with this minifilter.

Arguments:

    ConnectionCookie - unused

    OperationCode - An identifier describing what type of message this
        is.  These codes are defined by the MiniFilter.
    InputBuffer - A buffer containing input data, can be NULL if there
        is no input data.
    InputBufferSize - The size in bytes of the InputBuffer.
    OutputBuffer - A buffer provided by the application that originated
        the communication in which to store data to be returned to this
        application.
    OutputBufferSize - The size in bytes of the OutputBuffer.
    ReturnOutputBufferSize - The size in bytes of meaningful data
        returned in the OutputBuffer.

Return Value:

    Returns the status of processing the message.

--*/
{
    MINISPY_COMMAND command;
    NTSTATUS status;

    PAGED_CODE();

    UNREFERENCED_PARAMETER( ConnectionCookie );

    //
    //                      **** PLEASE READ ****
    //
    //  The INPUT and OUTPUT buffers are raw user mode addresses.  The filter
    //  manager has already done a ProbedForRead (on InputBuffer) and
    //  ProbedForWrite (on OutputBuffer) which guarentees they are valid
    //  addresses based on the access (user mode vs. kernel mode).  The
    //  minifilter does not need to do their own probe.
    //
    //  The filter manager is NOT doing any alignment checking on the pointers.
    //  The minifilter must do this themselves if they care (see below).
    //
    //  The minifilter MUST continue to use a try/except around any access to
    //  these buffers.
    //

    if ((InputBuffer != NULL) &&
        (InputBufferSize >= (FIELD_OFFSET(COMMAND_MESSAGE,Command) +
                             sizeof(MINISPY_COMMAND)))) {

        try  {

            //
            //  Probe and capture input message: the message is raw user mode
            //  buffer, so need to protect with exception handler
            //

            command = ((PCOMMAND_MESSAGE) InputBuffer)->Command;

        } except (SpyExceptionFilter( GetExceptionInformation(), TRUE )) {
        
            return GetExceptionCode();
        }

        switch (command) {

            case GetMiniSpyLog:

                //
                //  Return as many log records as can fit into the OutputBuffer
                //

                if ((OutputBuffer == NULL) || (OutputBufferSize == 0)) {

                    status = STATUS_INVALID_PARAMETER;
                    break;
                }

                //
                //  We want to validate that the given buffer is POINTER
                //  aligned.  But if this is a 64bit system and we want to
                //  support 32bit applications we need to be careful with how
                //  we do the check.  Note that the way SpyGetLog is written
                //  it actually does not care about alignment but we are
                //  demonstrating how to do this type of check.
                //

#if defined(_WIN64)

                if (IoIs32bitProcess( NULL )) {

                    //
                    //  Validate alignment for the 32bit process on a 64bit
                    //  system
                    //

                    if (!IS_ALIGNED(OutputBuffer,sizeof(ULONG))) {

                        status = STATUS_DATATYPE_MISALIGNMENT;
                        break;
                    }

                } else {

#endif

                    if (!IS_ALIGNED(OutputBuffer,sizeof(PVOID))) {

                        status = STATUS_DATATYPE_MISALIGNMENT;
                        break;
                    }

#if defined(_WIN64)

                }

#endif

                //
                //  Get the log record.
                //

                status = SpyGetLog( OutputBuffer,
                                    OutputBufferSize,
                                    ReturnOutputBufferLength );
                break;


            case GetMiniSpyVersion:

                //
                //  Return version of the MiniSpy filter driver.  Verify
                //  we have a valid user buffer including valid
                //  alignment
                //

                if ((OutputBufferSize < sizeof( MINISPYVER )) ||
                    (OutputBuffer == NULL)) {

                    status = STATUS_INVALID_PARAMETER;
                    break;
                }

                //
                //  Validate Buffer alignment.  If a minifilter cares about
                //  the alignment value of the buffer pointer they must do
                //  this check themselves.  Note that a try/except will not
                //  capture alignment faults.
                //

                if (!IS_ALIGNED(OutputBuffer,sizeof(ULONG))) {

                    status = STATUS_DATATYPE_MISALIGNMENT;
                    break;
                }

                //
                //  Protect access to raw user-mode output buffer with an
                //  exception handler
                //

                try {

                    ((PMINISPYVER)OutputBuffer)->Major = USBWL_MAJ_VERSION;
                    ((PMINISPYVER)OutputBuffer)->Minor = USBWL_MIN_VERSION;

                } except (SpyExceptionFilter( GetExceptionInformation(), TRUE )) {

                      return GetExceptionCode();
                }

                *ReturnOutputBufferLength = sizeof( MINISPYVER );
                status = STATUS_SUCCESS;
                break;

            default:
                status = STATUS_INVALID_PARAMETER;
                break;
        }

    } else {

        status = STATUS_INVALID_PARAMETER;
    }

    return status;
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
    PRECORD_LIST recordList;
    PFLT_FILE_NAME_INFORMATION nameInfo = NULL;
    UNICODE_STRING defaultName;
    PUNICODE_STRING nameToUse;
    NTSTATUS status;
    PUNICODE_STRING ecpDataToUse = NULL;
    UNICODE_STRING ecpData;
    WCHAR ecpDataBuffer[MAX_NAME_SPACE/sizeof(WCHAR)];

	if (Data && Data->Iopb && Data->Iopb->MajorFunction == IRP_MJ_CREATE)
	{
		//http://www.osronline.com/showThread.cfm?link=79773
		PFLT_IO_PARAMETER_BLOCK IopbPtr = Data->Iopb;
		PFLT_PARAMETERS ParameterPtr = &IopbPtr->Parameters;
		PIO_SECURITY_CONTEXT SecurityContextPtr = ParameterPtr->Create.SecurityContext;

		ULONG createDisposition = (Data->Iopb->Parameters.Create.Options >> 24) & 0x000000FF;
		BOOLEAN isNewFile = ((FILE_SUPERSEDE == createDisposition)
			|| (FILE_CREATE == createDisposition)
			|| (FILE_OPEN_IF == createDisposition)
			|| (FILE_OVERWRITE == createDisposition)
			|| (FILE_OVERWRITE_IF == createDisposition));

		ACCESS_MASK desiredAccess = SecurityContextPtr->DesiredAccess;
		BOOLEAN writeOperation = ((desiredAccess & (FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_SUPERSEDE | GENERIC_WRITE | FILE_WRITE_EA | FILE_APPEND_DATA | DELETE | WRITE_DAC | WRITE_OWNER)) ||
			(Data->Iopb->Parameters.Create.Options & FILE_DELETE_ON_CLOSE) || isNewFile);

		if (writeOperation) {
			Data->IoStatus.Status = STATUS_ACCESS_DENIED; //STATUS_CANCELLED; 
			Data->IoStatus.Information = 0;
			DbgPrint("IRP Major: %u \n", Data->Iopb->MajorFunction);
			DbgPrint("IRP Minor: %u \n", Data->Iopb->MinorFunction);
			return FLT_PREOP_COMPLETE;
		}
	}


    //
    //  Try and get a log record
    //

    recordList = SpyNewRecord();

    if (recordList) {

        //
        //  We got a log record, if there is a file object, get its name.
        //
        //  NOTE: By default, we use the query method
        //  FLT_FILE_NAME_QUERY_ALWAYS_ALLOW_CACHE_LOOKUP
        //  because MiniSpy would like to get the name as much as possible, but
        //  can cope if we can't retrieve a name.  For a debugging type filter,
        //  like Minispy, this is reasonable, but for most production filters
        //  who need names reliably, they should query the name at times when it
        //  is known to be safe and use the query method
        //  FLT_FILE_NAME_QUERY_DEFAULT.
        //

        if (FltObjects->FileObject != NULL) {

            status = FltGetFileNameInformation( Data,
                                                FLT_FILE_NAME_NORMALIZED |
                                                    UsbWlData.NameQueryMethod,
                                                &nameInfo );

        } else {

            //
            //  Can't get a name when there's no file object
            //

            status = STATUS_UNSUCCESSFUL;
        }

        //
        //  Use the name if we got it else use a default name
        //

        if (NT_SUCCESS( status )) {

            nameToUse = &nameInfo->Name;

            //
            //  Parse the name if requested
            //

            if (FlagOn( UsbWlData.DebugFlags, SPY_DEBUG_PARSE_NAMES )) {
                FltParseFileNameInformation( nameInfo );
            }

        } else {



            //
            //  We were unable to get the String safe routine to work on W2K
            //  Do it the old safe way
            //

            RtlInitUnicodeString( &defaultName, L"<NO NAME>" );
            nameToUse = &defaultName;
        }


        //
        //  Look for ECPs, but only if it's a create operation
        //


        if (Data->Iopb->MajorFunction == IRP_MJ_CREATE) {

            //
            //  Initialize an empty string to receive an ECP data dump
            //

            RtlInitEmptyUnicodeString( &ecpData,
                                       ecpDataBuffer,
                                       MAX_NAME_SPACE/sizeof(WCHAR) );

            //
            //  Parse any extra create parameters
            //

            SpyParseEcps( Data, recordList, &ecpData );

            ecpDataToUse = &ecpData;
        }

        //
        //  Store the name and ECP data (if any)
        //

        SpySetRecordNameAndEcpData( &(recordList->LogRecord), nameToUse, ecpDataToUse );

		/*if (Data->Iopb->MajorFunction == IRP_MJ_WRITE ||
			Data->Iopb->MajorFunction == IRP_MJ_CREATE ||
			Data->Iopb->MajorFunction == IRP_MJ_SET_INFORMATION ||
			Data->Iopb->MajorFunction == IRP_MJ_SET_EA ||
			Data->Iopb->MajorFunction == IRP_MJ_FLUSH_BUFFERS ||
			Data->Iopb->MajorFunction == IRP_MJ_SET_VOLUME_INFORMATION ||
			Data->Iopb->MajorFunction == IRP_MJ_SET_SECURITY ||
			Data->Iopb->MajorFunction == IRP_MJ_SET_QUOTA ||
			Data->Iopb->MajorFunction == IRP_MJ_ACQUIRE_FOR_SECTION_SYNCHRONIZATION ||
			Data->Iopb->MajorFunction == IRP_MJ_ACQUIRE_FOR_MOD_WRITE ||
			Data->Iopb->MajorFunction == IRP_MJ_ACQUIRE_FOR_CC_FLUSH ||
			Data->Iopb->MajorFunction == IRP_MJ_NETWORK_QUERY_OPEN ||
			Data->Iopb->MajorFunction == IRP_MJ_PREPARE_MDL_WRITE ||
			Data->Iopb->MajorFunction == IRP_MJ_MDL_WRITE_COMPLETE)*/


		// THIS BLOCKS ALL ACCESS:
		//if (Data->Iopb->MajorFunction == IRP_MJ_WRITE ||
		//Data->Iopb->MajorFunction == IRP_MJ_CREATE ||
		//Data->Iopb->MajorFunction == IRP_MJ_SET_INFORMATION ||
		//Data->Iopb->MajorFunction == IRP_MJ_SET_EA ||
		//Data->Iopb->MajorFunction == IRP_MJ_FLUSH_BUFFERS ||
		//Data->Iopb->MajorFunction == IRP_MJ_SET_VOLUME_INFORMATION ||
		//Data->Iopb->MajorFunction == IRP_MJ_SET_SECURITY ||
		//Data->Iopb->MajorFunction == IRP_MJ_SET_QUOTA ||
		//Data->Iopb->MajorFunction == IRP_MJ_ACQUIRE_FOR_SECTION_SYNCHRONIZATION ||
		//Data->Iopb->MajorFunction == IRP_MJ_ACQUIRE_FOR_MOD_WRITE ||
		//Data->Iopb->MajorFunction == IRP_MJ_ACQUIRE_FOR_CC_FLUSH ||
		//Data->Iopb->MajorFunction == IRP_MJ_PREPARE_MDL_WRITE ||
		//Data->Iopb->MajorFunction == IRP_MJ_MDL_WRITE_COMPLETE)
		//{
		//	//https://www.osronline.com/showthread.cfm?link=236160
		//	if (!FlagOn(Data->Iopb->IrpFlags, IRP_PAGING_IO)) {
		//		Data->IoStatus.Status = STATUS_CANCELLED; //STATUS_ACCESS_DENIED; 
		//		Data->IoStatus.Information = 0; 
		//		DbgPrint("IRP Major: %d \n", Data->Iopb->MajorFunction);
		//		DbgPrint("IRP Minor: %d \n", Data->Iopb->MinorFunction);
		//		return FLT_PREOP_COMPLETE;
		//	}
		//}

		


		//if (Data->Iopb->MajorFunction == IRP_MJ_WRITE ||
		//	Data->Iopb->MajorFunction == IRP_MJ_SET_INFORMATION ||
		//	Data->Iopb->MajorFunction == IRP_MJ_ACQUIRE_FOR_SECTION_SYNCHRONIZATION)
		//{
		//	DbgPrint("IRP Requested: %d \n", Data->Iopb->MajorFunction);
		//	Data->IoStatus.Status = STATUS_CANCELLED; //STATUS_ACCESS_DENIED; 
		//	Data->IoStatus.Information = 0; 
		//	return FLT_PREOP_COMPLETE;
		//}

        //
        //  Release the name information structure (if defined)
        //

        if (NULL != nameInfo) {

            FltReleaseFileNameInformation( nameInfo );
        }

        //
        //  Set all of the operation information into the record
        //

        SpyLogPreOperationData( Data, FltObjects, recordList );

        //
        //  Pass the record to our completions routine and return that
        //  we want our completion routine called.
        //

        if (Data->Iopb->MajorFunction == IRP_MJ_SHUTDOWN) {

            //
            //  Since completion callbacks are not supported for
            //  this operation, do the completion processing now
            //

            WlPostOperationCallback( Data,
                                      FltObjects,
                                      recordList,
                                      0 );

            returnStatus = FLT_PREOP_SUCCESS_NO_CALLBACK;

        } else {

            *CompletionContext = recordList;
            returnStatus = FLT_PREOP_SUCCESS_WITH_CALLBACK;
        }
    }

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
    PRECORD_LIST recordList;
    PRECORD_LIST reparseRecordList = NULL;
    PLOG_RECORD reparseLogRecord;
    PFLT_TAG_DATA_BUFFER tagData;
    ULONG copyLength;

    UNREFERENCED_PARAMETER( FltObjects );

    recordList = (PRECORD_LIST)CompletionContext;

    //
    //  If our instance is in the process of being torn down don't bother to
    //  log this record, free it now.
    //

    if (FlagOn(Flags,FLTFL_POST_OPERATION_DRAINING)) {

        SpyFreeRecord( recordList );
        return FLT_POSTOP_FINISHED_PROCESSING;
    }

    //
    //  Set completion information into the record
    //

    SpyLogPostOperationData( Data, recordList );

    //
    //  Log reparse tag information if specified.
    //

    tagData = Data->TagData;
    if (tagData) {

        reparseRecordList = SpyNewRecord();

        if (reparseRecordList) {

            //
            //  only copy the DATA portion of the information
            //

            RtlCopyMemory( &reparseRecordList->LogRecord.Data,
                           &recordList->LogRecord.Data,
                           sizeof(RECORD_DATA) );

            reparseLogRecord = &reparseRecordList->LogRecord;

            copyLength = FLT_TAG_DATA_BUFFER_HEADER_SIZE + tagData->TagDataLength;

            if(copyLength > MAX_NAME_SPACE) {

                copyLength = MAX_NAME_SPACE;
            }

            //
            //  Copy reparse data
            //

            RtlCopyMemory(
                &reparseRecordList->LogRecord.Name[0],
                tagData,
                copyLength
                );

            reparseLogRecord->RecordType |= RECORD_TYPE_FILETAG;
            reparseLogRecord->Length += (ULONG) ROUND_TO_SIZE( copyLength, sizeof( PVOID ) );
        }
    }

    //
    //  Send the logged information to the user service.
    //

    SpyLog( recordList );

    if (reparseRecordList) {

        SpyLog( reparseRecordList );
    }

    //
    //  For creates within a transaction enlist in the transaction
    //  if we haven't already done.
    //

    if ((FltObjects->Transaction != NULL) &&
        (Data->Iopb->MajorFunction == IRP_MJ_CREATE) &&
        (Data->IoStatus.Status == STATUS_SUCCESS)) {

        //
        //  Enlist in the transaction.
        //

        SpyEnlistInTransaction( FltObjects );
    }

    return FLT_POSTOP_FINISHED_PROCESSING;
}


NTSTATUS
SpyEnlistInTransaction (
    _In_ PCFLT_RELATED_OBJECTS FltObjects
    )
/*++

Routine Description

    Minispy calls this function to enlist in a transaction of interest. 

Arguments

    FltObjects - Contains parameters required to enlist in a transaction.

Return value

    Returns STATUS_SUCCESS if we were able to successfully enlist in a new transcation or if we
    were already enlisted in the transaction. Returns an appropriate error code on a failure.
    
--*/
{

#if USBWL_VISTA

    PMINISPY_TRANSACTION_CONTEXT transactionContext = NULL;
    PMINISPY_TRANSACTION_CONTEXT oldTransactionContext = NULL;
    PRECORD_LIST recordList;
    NTSTATUS status;
    static ULONG Sequence=1;

    //
    //  This code is only built in the Vista environment, but
    //  we need to ensure this binary still runs down-level.  Return
    //  at this point if the transaction dynamic imports were not found.
    //
    //  If we find FltGetTransactionContext, we assume the other
    //  transaction APIs are also present.
    //

    if (NULL == UsbWlData.PFltGetTransactionContext) {

        return STATUS_SUCCESS;
    }

    //
    //  Try to get our context for this transaction. If we get
    //  one we have already enlisted in this transaction.
    //

    status = (*UsbWlData.PFltGetTransactionContext)( FltObjects->Instance,
                                                       FltObjects->Transaction,
                                                       &transactionContext );

    if (NT_SUCCESS( status )) {

        // 
        //  Check if we have already enlisted in the transaction. 
        //

        if (FlagOn(transactionContext->Flags, MINISPY_ENLISTED_IN_TRANSACTION)) {

            //
            //  FltGetTransactionContext puts a reference on the context. Release
            //  that now and return success.
            //
            
            FltReleaseContext( transactionContext );
            return STATUS_SUCCESS;
        }

        //
        //  If we have not enlisted then we need to try and enlist in the transaction.
        //
        
        goto ENLIST_IN_TRANSACTION;
    }

    //
    //  If the context does not exist create a new one, else return the error
    //  status to the caller.
    //

    if (status != STATUS_NOT_FOUND) {

        return status;
    }

    //
    //  Allocate a transaction context.
    //

    status = FltAllocateContext( FltObjects->Filter,
                                 FLT_TRANSACTION_CONTEXT,
                                 sizeof(USBWL_TRANSACTION_CONTEXT),
                                 PagedPool,
                                 &transactionContext );

    if (!NT_SUCCESS( status )) {

        return status;
    }

    //
    //  Set the context into the transaction
    //

    RtlZeroMemory(transactionContext, sizeof(USBWL_TRANSACTION_CONTEXT));
    transactionContext->Count = Sequence++;

    FLT_ASSERT( UsbWlData.PFltSetTransactionContext );

    status = (*UsbWlData.PFltSetTransactionContext)( FltObjects->Instance,
                                                       FltObjects->Transaction,
                                                       FLT_SET_CONTEXT_KEEP_IF_EXISTS,
                                                       transactionContext,
                                                       &oldTransactionContext );

    if (!NT_SUCCESS( status )) {

        FltReleaseContext( transactionContext );    //this will free the context

        if (status != STATUS_FLT_CONTEXT_ALREADY_DEFINED) {

            return status;
        }

        FLT_ASSERT(oldTransactionContext != NULL);
        
        if (FlagOn(oldTransactionContext->Flags, MINISPY_ENLISTED_IN_TRANSACTION)) {

            //
            //  If this context is already enlisted then release the reference
            //  which FltSetTransactionContext put on it and return success.
            //
            
            FltReleaseContext( oldTransactionContext );
            return STATUS_SUCCESS;
        }

        //
        //  If we found an existing transaction then we should try and
        //  enlist in it. There is a race here in which the thread 
        //  which actually set the transaction context may fail to 
        //  enlist in the transaction and delete it later. It might so
        //  happen that we picked up a reference to that context here
        //  and successfully enlisted in that transaction. For now
        //  we have chosen to ignore this scenario.
        //

        //
        //  If we are not enlisted then assign the right transactionContext
        //  and attempt enlistment.
        //

        transactionContext = oldTransactionContext;            
    }

ENLIST_IN_TRANSACTION: 

    //
    //  Enlist on this transaction for notifications.
    //

    FLT_ASSERT( UsbWlData.PFltEnlistInTransaction );

    status = (*UsbWlData.PFltEnlistInTransaction)( FltObjects->Instance,
                                                     FltObjects->Transaction,
                                                     transactionContext,
                                                     FLT_MAX_TRANSACTION_NOTIFICATIONS );

    //
    //  If the enlistment failed we might have to delete the context and remove
    //  our count.
    //

    if (!NT_SUCCESS( status )) {

        //
        //  If the error is that we are already enlisted then we do not need
        //  to delete the context. Otherwise we have to delete the context
        //  before releasing our reference.
        //
        
        if (status == STATUS_FLT_ALREADY_ENLISTED) {

            status = STATUS_SUCCESS;

        } else {

            //
            //  It is worth noting that only the first caller of
            //  FltDeleteContext will remove the reference added by
            //  filter manager when the context was set.
            //
            
            FltDeleteContext( transactionContext );
        }
        
        FltReleaseContext( transactionContext );
        return status;
    }

    //
    //  Set the flag so that future enlistment efforts know that we
    //  successfully enlisted in the transaction.
    //

    SetFlagInterlocked( &transactionContext->Flags, MINISPY_ENLISTED_IN_TRANSACTION );
    
    //
    //  The operation succeeded, remove our count
    //

    FltReleaseContext( transactionContext );

    //
    //  Log a record that a new transaction has started.
    //

    recordList = SpyNewRecord();

    if (recordList) {

        SpyLogTransactionNotify( FltObjects, recordList, 0 );

        //
        //  Send the logged information to the user service.
        //

        SpyLog( recordList );
    }

#endif // USBWL_VISTA

    return STATUS_SUCCESS;
}


#if USBWL_VISTA

NTSTATUS
WlKtmNotificationCallback (
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _In_ PFLT_CONTEXT TransactionContext,
    _In_ ULONG TransactionNotification
    )
{
    PRECORD_LIST recordList;

    UNREFERENCED_PARAMETER( TransactionContext );

    //
    //  Try and get a log record
    //

    recordList = SpyNewRecord();

    if (recordList) {

        SpyLogTransactionNotify( FltObjects, recordList, TransactionNotification );

        //
        //  Send the logged information to the user service.
        //

        SpyLog( recordList );
    }

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


LONG
SpyExceptionFilter (
    _In_ PEXCEPTION_POINTERS ExceptionPointer,
    _In_ BOOLEAN AccessingUserBuffer
    )
/*++

Routine Description:

    Exception filter to catch errors touching user buffers.

Arguments:

    ExceptionPointer - The exception record.

    AccessingUserBuffer - If TRUE, overrides FsRtlIsNtStatusExpected to allow
                          the caller to munge the error to a desired status.

Return Value:

    EXCEPTION_EXECUTE_HANDLER - If the exception handler should be run.

    EXCEPTION_CONTINUE_SEARCH - If a higher exception handler should take care of
                                this exception.

--*/
{
    NTSTATUS Status;

    Status = ExceptionPointer->ExceptionRecord->ExceptionCode;

    //
    //  Certain exceptions shouldn't be dismissed within the namechanger filter
    //  unless we're touching user memory.
    //

    if (!FsRtlIsNtstatusExpected( Status ) &&
        !AccessingUserBuffer) {

        return EXCEPTION_CONTINUE_SEARCH;
    }

    return EXCEPTION_EXECUTE_HANDLER;
}


