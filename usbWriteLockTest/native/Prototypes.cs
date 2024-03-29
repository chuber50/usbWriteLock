﻿using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using usbWriteLockTest.native.constant;

namespace usbWriteLockTest.native
{
    // prototypes collected from http://www.pinvoke.net/
    class Prototypes
    {
        public const uint InvalidHandleValue = 0xFFFFFFFF;

        public const uint InvalidSetFilePointer = 0xFFFFFFFF;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern SafeFileHandle CreateFile(
            string lpFileName,
            EDesiredAccess dwDesiredAccess,
            EShareMode dwShareMode,
            IntPtr lpSecurityAttributes,
            ECreationDisposition dwCreationDisposition,
            EFlagsAndAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern Int32 CloseHandle(
            SafeFileHandle hObject);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool ReadFile(
            SafeFileHandle hFile,
            Byte[] aBuffer,
            UInt32 cbToRead,
            ref UInt32 cbThatWereRead,
            IntPtr pOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern unsafe bool ReadFile(
            SafeFileHandle hFile,
            byte* pBuffer,
            uint NumberOfBytesToRead,
            uint* pNumberOfBytesRead,
            IntPtr Overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteFile(
            SafeFileHandle hFile,
            Byte[] aBuffer,
            UInt32 cbToWrite,
            ref UInt32 cbThatWereWritten,
            IntPtr pOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern unsafe bool WriteFile(
            SafeFileHandle hFile,
            byte* pBuffer,
            uint NumberOfBytesToWrite,
            uint* pNumberOfBytesWritten,
            IntPtr Overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 SetFilePointer(
            SafeFileHandle hFile,
            Int32 cbDistanceToMove,
            IntPtr pDistanceToMoveHigh,
            EMoveMethod fMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetFilePointerEx(
            SafeFileHandle hFile, ulong liDistanceToMove,
            out ulong lpNewFilePointer, uint dwMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetEndOfFile(
            SafeFileHandle hFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 GetFileSize(
            SafeFileHandle hFile,
            IntPtr pFileSizeHigh);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            EioControlCode ioControlCode,
            [MarshalAs(UnmanagedType.AsAny)] [In] object inBuffer,
            uint nInBufferSize,
            [MarshalAs(UnmanagedType.AsAny)] [Out] object outBuffer,
            uint nOutBufferSize,
            ref uint pBytesReturned,
            [In] IntPtr overlapped
        );

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool CreateHardLink(
            string lpFileName,
            string lpExistingFileName,
            IntPtr lpSecurityAttributes
        );
    }
}
