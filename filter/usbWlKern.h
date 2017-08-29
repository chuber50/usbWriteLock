/*++

Copyright (c) 1989-2002  Microsoft Corporation

Module Name:

    mspyKern.h

Abstract:
    Header file which contains the structures, type definitions,
    constants, global variables and function prototypes that are
    only visible within the kernel.

Environment:

    Kernel mode

--*/
#ifndef __USBWLKERN_H__
#define __USBWLKERN_H__

#include <fltKernel.h>
//#include <dontuse.h>
#include <suppress.h>
#include "usbwl.h"

#pragma prefast(disable:__WARNING_ENCODE_MEMBER_FUNCTION_POINTER, "Not valid for kernel mode drivers")

//
//  Memory allocation tag
//

#define WL_TAG 'ypSM'

//
//  Win8 define for support of NPFS/MSFS
//  Win7 define for support of new ECPs.
//  Vista define for including transaction support,
//  older ECPs
//

#define USBWL_WIN8     (NTDDI_VERSION >= NTDDI_WIN8)
#define USBWL_WIN7     (NTDDI_VERSION >= NTDDI_WIN7)
#define USBWL_VISTA    (NTDDI_VERSION >= NTDDI_VISTA)
#define USBWL_NOT_W2K  (OSVER(NTDDI_VERSION) > NTDDI_WIN2K)

//
//  Define callback types for Vista
//

#if USBWL_VISTA

//
//  Dynamically imported Filter Mgr APIs
//

typedef NTSTATUS
(*PFLT_SET_TRANSACTION_CONTEXT)(
    _In_ PFLT_INSTANCE Instance,
    _In_ PKTRANSACTION Transaction,
    _In_ FLT_SET_CONTEXT_OPERATION Operation,
    _In_ PFLT_CONTEXT NewContext,
    _Outptr_opt_ PFLT_CONTEXT *OldContext
    );

typedef NTSTATUS
(*PFLT_GET_TRANSACTION_CONTEXT)(
    _In_ PFLT_INSTANCE Instance,
    _In_ PKTRANSACTION Transaction,
    _Outptr_ PFLT_CONTEXT *Context
    );

typedef NTSTATUS
(*PFLT_ENLIST_IN_TRANSACTION)(
    _In_ PFLT_INSTANCE Instance,
    _In_ PKTRANSACTION Transaction,
    _In_ PFLT_CONTEXT TransactionContext,
    _In_ NOTIFICATION_MASK NotificationMask
    );

//
// Flags for the known ECPs
//

#define ECP_TYPE_FLAG_PREFETCH                   0x00000001

#if USBWL_WIN7

#define ECP_TYPE_FLAG_OPLOCK_KEY                 0x00000002
#define ECP_TYPE_FLAG_NFS                        0x00000004
#define ECP_TYPE_FLAG_SRV                        0x00000008

#endif

#define ADDRESS_STRING_BUFFER_SIZE          64

//
//  Enumerate the ECPs MiniSpy supports
//

typedef enum _ECP_TYPE {

    EcpPrefetchOpen,
    EcpOplockKey,
    EcpNfsOpen,
    EcpSrvOpen,

    NumKnownEcps

} ECP_TYPE;

#endif

//---------------------------------------------------------------------------
//      Global variables
//---------------------------------------------------------------------------

typedef struct _MINISPY_DATA {

    //
    //  The object that identifies this driver.
    //

    PDRIVER_OBJECT DriverObject;

    //
    //  The filter that results from a call to
    //  FltRegisterFilter.
    //

    PFLT_FILTER Filter;

    //
    //  Server port: user mode connects to this port
    //

    PFLT_PORT ServerPort;

    //
    //  Client connection port: only one connection is allowed at a time.,
    //

    PFLT_PORT ClientPort;

    //
    //  List of buffers with data to send to user mode.
    //

    KSPIN_LOCK OutputBufferLock;
    LIST_ENTRY OutputBufferList;

    //
    //  Lookaside list used for allocating buffers.
    //

    NPAGED_LOOKASIDE_LIST FreeBufferList;

    //
    //  Variables used to throttle how many records buffer we can use
    //

    LONG MaxRecordsToAllocate;
    __volatile LONG RecordsAllocated;

    //
    //  static buffer used for sending an "out-of-memory" message
    //  to user mode.
    //

    __volatile LONG StaticBufferInUse;

    //
    //  We need to make sure this buffer aligns on a PVOID boundary because
    //  minispy casts this buffer to a RECORD_LIST structure.
    //  That can cause alignment faults unless the structure starts on the
    //  proper PVOID boundary
    //

    PVOID OutOfMemoryBuffer[RECORD_SIZE/sizeof( PVOID )];

    //
    //  Variable and lock for maintaining LogRecord sequence numbers.
    //

    __volatile LONG LogSequenceNumber;

    //
    //  The name query method to use.  By default, it is set to
    //  FLT_FILE_NAME_QUERY_ALWAYS_ALLOW_CACHE_LOOKUP, but it can be overridden
    //  by a setting in the registery.
    //

    ULONG NameQueryMethod;

    //
    //  Global debug flags
    //

    ULONG DebugFlags;

#if USBWL_VISTA

    //
    //  Dynamically imported Filter Mgr APIs
    //

    PFLT_SET_TRANSACTION_CONTEXT PFltSetTransactionContext;

    PFLT_GET_TRANSACTION_CONTEXT PFltGetTransactionContext;

    PFLT_ENLIST_IN_TRANSACTION PFltEnlistInTransaction;

#endif

} USBWL_DATA, *PUSBWL_DATA;


//
//  Defines the minispy context structure
//

typedef struct _MINISPY_TRANSACTION_CONTEXT {
    ULONG Flags;
    ULONG Count;

}USBWL_TRANSACTION_CONTEXT, *PMINISPY_TRANSACTION_CONTEXT;

//
//  This macro below is used to set the flags field in minispy's
//  USBWL_TRANSACTION_CONTEXT structure once it has been
//  successfully enlisted in the transaction.
//

#define MINISPY_ENLISTED_IN_TRANSACTION 0x01

//
//  Minispy's global variables
//

extern USBWL_DATA UsbWlData;

#define DEFAULT_MAX_RECORDS_TO_ALLOCATE     500
#define MAX_RECORDS_TO_ALLOCATE             L"MaxRecords"

#define DEFAULT_NAME_QUERY_METHOD           FLT_FILE_NAME_QUERY_ALWAYS_ALLOW_CACHE_LOOKUP
#define NAME_QUERY_METHOD                   L"NameQueryMethod"

//
//  DebugFlag values
//

#define SPY_DEBUG_PARSE_NAMES   0x00000001

//---------------------------------------------------------------------------
//  Registration structure
//---------------------------------------------------------------------------

extern const FLT_REGISTRATION FilterRegistration;

//---------------------------------------------------------------------------
//  Function prototypes
//---------------------------------------------------------------------------

FLT_PREOP_CALLBACK_STATUS
WlPreOperationCallback (
    _Inout_ PFLT_CALLBACK_DATA Data,
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _Flt_CompletionContext_Outptr_ PVOID *CompletionContext
    );

FLT_POSTOP_CALLBACK_STATUS
WlPostOperationCallback (
    _Inout_ PFLT_CALLBACK_DATA Data,
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _In_ PVOID CompletionContext,
    _In_ FLT_POST_OPERATION_FLAGS Flags
    );

NTSTATUS
WlKtmNotificationCallback (
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _In_ PFLT_CONTEXT TransactionContext,
    _In_ ULONG TransactionNotification
    );

NTSTATUS
WlFilterUnload (
    _In_ FLT_FILTER_UNLOAD_FLAGS Flags
    );

NTSTATUS
WlQueryTeardown (
    _In_ PCFLT_RELATED_OBJECTS FltObjects,
    _In_ FLT_INSTANCE_QUERY_TEARDOWN_FLAGS Flags
    );

VOID
WlDeleteTxfContext (
    _Inout_ PFLT_CONTEXT  Context,
    _In_ FLT_CONTEXT_TYPE  ContextType
    );

#endif  //__MSPYKERN_H__

