using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using usbWriteLockTest.native.constant;

namespace usbWriteLockTest.native
{
    class Prototypes
    {
        public const uint InvalidHandleValue = 0xFFFFFFFF;

        public const uint InvalidSetFilePointer = 0xFFFFFFFF;

        // Use interop to call the CreateFile function.
        // For more information about CreateFile,
        // see the unmanaged MSDN reference library.
        [DllImport("kernel32.dll", SetLastError = true)]
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
        internal static extern bool WriteFile(
            SafeFileHandle hFile,
            Byte[] aBuffer,
            UInt32 cbToWrite,
            ref UInt32 cbThatWereWritten,
            IntPtr pOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 SetFilePointer(
            SafeFileHandle hFile,
            Int32 cbDistanceToMove,
            IntPtr pDistanceToMoveHigh,
            EMoveMethod fMoveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetEndOfFile(
            SafeFileHandle hFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 GetFileSize(
            SafeFileHandle hFile,
            IntPtr pFileSizeHigh);

        [DllImport("Kernel32.dll", SetLastError = false, CharSet = CharSet.Auto)]
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
    }
}
