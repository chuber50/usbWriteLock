/*++

Copyright (c) 1989-2002  Microsoft Corporation

Module Name:

    RegistrationData.c

Abstract:

    This filters registration information.  Note that this is in a unique file
    so it could be set into the INIT section.

Environment:

    Kernel mode

--*/

#include "usbWlKern.h"

//---------------------------------------------------------------------------
//  Registration information for FLTMGR.
//---------------------------------------------------------------------------

//
//  Tells the compiler to define all following DATA and CONSTANT DATA to
//  be placed in the INIT segment.
//

#ifdef ALLOC_DATA_PRAGMA
    #pragma data_seg("INIT")
    #pragma const_seg("INIT")
#endif

CONST FLT_OPERATION_REGISTRATION Callbacks[] = {
    { IRP_MJ_CREATE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_CREATE_NAMED_PIPE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_CLOSE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_READ,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_WRITE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_QUERY_INFORMATION,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_SET_INFORMATION,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_QUERY_EA,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_SET_EA,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_FLUSH_BUFFERS,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_QUERY_VOLUME_INFORMATION,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_SET_VOLUME_INFORMATION,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_DIRECTORY_CONTROL,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_FILE_SYSTEM_CONTROL,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_DEVICE_CONTROL,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_INTERNAL_DEVICE_CONTROL,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_SHUTDOWN,
      0,
      WlPreOperationCallback,
      NULL },                           //post operation callback not supported

    { IRP_MJ_LOCK_CONTROL,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_CLEANUP,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_CREATE_MAILSLOT,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_QUERY_SECURITY,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_SET_SECURITY,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_QUERY_QUOTA,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_SET_QUOTA,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_PNP,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_ACQUIRE_FOR_SECTION_SYNCHRONIZATION,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_RELEASE_FOR_SECTION_SYNCHRONIZATION,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_ACQUIRE_FOR_MOD_WRITE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_RELEASE_FOR_MOD_WRITE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_ACQUIRE_FOR_CC_FLUSH,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_RELEASE_FOR_CC_FLUSH,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

/*    { IRP_MJ_NOTIFY_STREAM_FILE_OBJECT,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },*/

    { IRP_MJ_FAST_IO_CHECK_IF_POSSIBLE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_NETWORK_QUERY_OPEN,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_MDL_READ,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_MDL_READ_COMPLETE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_PREPARE_MDL_WRITE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_MDL_WRITE_COMPLETE,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_VOLUME_MOUNT,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_VOLUME_DISMOUNT,
      0,
      WlPreOperationCallback,
      WlPostOperationCallback },

    { IRP_MJ_OPERATION_END }
};

const FLT_CONTEXT_REGISTRATION Contexts[] = {

#if USBWL_VISTA

    { FLT_TRANSACTION_CONTEXT,
      0,
      WlDeleteTxfContext,
      sizeof(USBWL_TRANSACTION_CONTEXT),
      'ypsM' },

#endif // USBWL_VISTA

    { FLT_CONTEXT_END }
};

//
//  This defines what we want to filter with FltMgr
//

CONST FLT_REGISTRATION FilterRegistration = {

    sizeof(FLT_REGISTRATION),               //  Size
    FLT_REGISTRATION_VERSION,               //  Version   
#if USBWL_WIN8 
    FLTFL_REGISTRATION_SUPPORT_NPFS_MSFS,   //  Flags
#else
    0,                                      //  Flags
#endif // USBWL_WIN8

    Contexts,                               //  Context
    Callbacks,                              //  Operation callbacks

    WlFilterUnload,                        //  FilterUnload

    NULL,                                   //  InstanceSetup
    WlQueryTeardown,                       //  InstanceQueryTeardown
    NULL,                                   //  InstanceTeardownStart
    NULL,                                   //  InstanceTeardownComplete

    NULL,                                   //  GenerateFileName
    NULL,                                   //  GenerateDestinationFileName
    NULL                                    //  NormalizeNameComponent

#if USBWL_VISTA

    ,
    WlKtmNotificationCallback              //  KTM notification callback

#endif // USBWL_VISTA

};


//
//  Tells the compiler to restore the given section types back to their previous
//  section definition.
//

#ifdef ALLOC_DATA_PRAGMA
    #pragma data_seg()
    #pragma const_seg()
#endif

