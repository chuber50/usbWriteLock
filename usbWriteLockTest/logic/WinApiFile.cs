using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using usbWriteLockTest.native;
using usbWriteLockTest.native.constant;
using static usbWriteLockTest.native.Prototypes;

namespace usbWriteLockTest.logic
{
    public class WinApiFile : IDisposable
    {

        /* ---------------------------------------------------------
         * private members
         * ------------------------------------------------------ */
        private SafeFileHandle _hFile = null;
        private string _sFileName = "";
        private bool _fDisposed;

        /* ---------------------------------------------------------
         * properties
         * ------------------------------------------------------ */
        public bool IsOpen { get { return (_hFile != null); } }
        public SafeFileHandle Handle { get { return _hFile; } }
        public string FileName
        {
            get { return _sFileName; }
            set
            {
                _sFileName = (value ?? "").Trim();
                if (_sFileName.Length == 0)
                    CloseHandle(_hFile);
            }
        }
        public int FileLength
        {
            get
            {
                return (_hFile != null) ? (int)GetFileSize(_hFile,
                 IntPtr.Zero) : 0;
            }
            set
            {
                if (_hFile == null)
                    return;
                MoveFilePointer(value, EMoveMethod.FILE_BEGIN);
                if (!SetEndOfFile(_hFile))
                    ThrowLastWin32Err();
            }
        }

        /* ---------------------------------------------------------
         * Constructors
         * ------------------------------------------------------ */

        public WinApiFile(string sFileName,
         EDesiredAccess fDesiredAccess)
        {
            FileName = sFileName;
            Open(fDesiredAccess);
        }
        public WinApiFile(string sFileName,
         EDesiredAccess fDesiredAccess,
         ECreationDisposition fCreationDisposition)
        {
            FileName = sFileName;
            Open(fDesiredAccess, fCreationDisposition);
        }

        /* ---------------------------------------------------------
         * Open/Close
         * ------------------------------------------------------ */

        public void Open(
         EDesiredAccess fDesiredAccess)
        {
            Open(fDesiredAccess, ECreationDisposition.OPEN_EXISTING);
        }

        public void Open(
         EDesiredAccess fDesiredAccess,
         ECreationDisposition fCreationDisposition)
        {
            EShareMode fShareMode;
            if (fDesiredAccess == EDesiredAccess.GENERIC_READ)
            {
                fShareMode = EShareMode.FILE_SHARE_WRITE;
            }
            else
            {
                fShareMode = EShareMode.FILE_SHARE_NONE;
            }
            Open(fDesiredAccess, fShareMode, fCreationDisposition, 0);
        }

        public void Open(
         EDesiredAccess fDesiredAccess,
         EShareMode fShareMode,
         ECreationDisposition fCreationDisposition,
         EFlagsAndAttributes fFlagsAndAttributes)
        {

            if (_sFileName.Length == 0)
                throw new ArgumentNullException("FileName");
            _hFile = CreateFile(_sFileName, fDesiredAccess, fShareMode,
             IntPtr.Zero, fCreationDisposition, fFlagsAndAttributes,
             IntPtr.Zero);
            if (_hFile.IsInvalid)
            {
                _hFile = null;
                ThrowLastWin32Err();
            }
            _fDisposed = false;

        }

        public void Close()
        {
            if (_hFile == null)
                return;
            _hFile.Close();
            _hFile = null;
            _fDisposed = true;
        }

        public void Lock()
        {
            if (_sFileName.Length == 0)
                throw new ArgumentNullException("FileName");
            bool dismounted = DeviceIoControl(
                _hFile,            // handle to a volume
                (DWORD)FSCTL_DISMOUNT_VOLUME,   // dwIoControlCode
                NULL,                        // lpInBuffer
                0,                           // nInBufferSize
                NULL,                        // lpOutBuffer
                0,                           // nOutBufferSize
                (LPDWORD)lpBytesReturned,   // number of bytes returned
                (LPOVERLAPPED)lpOverlapped  // OVERLAPPED structure
            );
        }

        /* ---------------------------------------------------------
         * Move file pointer
         * ------------------------------------------------------ */

        public void MoveFilePointer(int cbToMove)
        {
            MoveFilePointer(cbToMove, EMoveMethod.FILE_CURRENT);
        }

        public void MoveFilePointer(int cbToMove,
         EMoveMethod fMoveMethod)
        {
            if (_hFile != null)
                if (SetFilePointer(_hFile, cbToMove, IntPtr.Zero,
                 fMoveMethod) == INVALID_SET_FILE_POINTER)
                    ThrowLastWin32Err();
        }

        public int FilePointer
        {
            get
            {
                return (_hFile != null) ? (int)SetFilePointer(_hFile, 0,
                 IntPtr.Zero, EMoveMethod.FILE_CURRENT) : 0;
            }
            set
            {
                MoveFilePointer(value);
            }
        }

        /* ---------------------------------------------------------
         * Read and Write
         * ------------------------------------------------------ */

        public uint Read(byte[] buffer, uint cbToRead)
        {
            // returns bytes read
            uint cbThatWereRead = 0;
            if (!ReadFile(_hFile, buffer, cbToRead,
             ref cbThatWereRead, IntPtr.Zero))
                ThrowLastWin32Err();
            return cbThatWereRead;
        }

        public uint Write(byte[] buffer, uint cbToWrite)
        {
            // returns bytes read
            uint cbThatWereWritten = 0;
            if (!WriteFile(_hFile, buffer, cbToWrite,
             ref cbThatWereWritten, IntPtr.Zero))
                ThrowLastWin32Err();
            return cbThatWereWritten;
        }

        /* ---------------------------------------------------------
         * IDisposable Interface
         * ------------------------------------------------------ */
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool fDisposing)
        {
            if (!_fDisposed)
            {
                if (fDisposing)
                {
                    if (_hFile != null)
                        _hFile.Dispose();
                    _fDisposed = true;
                }
            }
        }

        ~WinApiFile()
        {
            Dispose(false);
        }

        public void ThrowLastWin32Err()
        {
            Marshal.ThrowExceptionForHR(
                Marshal.GetHRForLastWin32Error());
        }
    }
