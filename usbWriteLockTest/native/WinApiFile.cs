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
        public bool isOpen
        {
            get { return (_hFile != null); }
        }

        public SafeFileHandle handle
        {
            get { return _hFile; }
        }

        public string fileName
        {
            get { return _sFileName; }
            set
            {
                _sFileName = (value ?? "").Trim();
                if (_sFileName.Length == 0)
                    CloseHandle(_hFile);
            }
        }

        public int fileLength
        {
            get
            {
                return (_hFile != null)
                    ? (int) GetFileSize(_hFile,
                        IntPtr.Zero)
                    : 0;
            }
            set
            {
                if (_hFile == null)
                    return;
                moveFilePointer(value, EMoveMethod.FileBegin);
                if (!SetEndOfFile(_hFile))
                    throwLastWin32Err();
            }
        }

        /* ---------------------------------------------------------
         * Constructors
         * ------------------------------------------------------ */

        public WinApiFile(string sFileName,
            EDesiredAccess fDesiredAccess)
        {
            fileName = sFileName;
            open(fDesiredAccess);
        }

        public WinApiFile(string sFileName,
            EDesiredAccess fDesiredAccess,
            ECreationDisposition fCreationDisposition)
        {
            fileName = sFileName;
            open(fDesiredAccess, fCreationDisposition);
        }

        /* ---------------------------------------------------------
         * Open/Close
         * ------------------------------------------------------ */

        public void open(
            EDesiredAccess fDesiredAccess)
        {
            open(fDesiredAccess, ECreationDisposition.OpenExisting);
        }

        public void open(
            EDesiredAccess fDesiredAccess,
            ECreationDisposition fCreationDisposition)
        {
            EShareMode fShareMode;
            if (fDesiredAccess == EDesiredAccess.GenericRead)
            {
                fShareMode = EShareMode.FileShareWrite;
            }
            else
            {
                fShareMode = EShareMode.FileShareNone;
            }
            open(fDesiredAccess, fShareMode, fCreationDisposition, 0);
        }

        public void open(
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
                throwLastWin32Err();
            }
            _fDisposed = false;

        }

        public void close()
        {
            if (_hFile == null)
                return;
            _hFile.Close();
            _hFile = null;
            _fDisposed = true;
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/aa364562.aspx
        //http://www.pinvoke.net/default.aspx/kernel32.deviceiocontrol
        public void Lock()
        {
            if (_sFileName.Length == 0)
                throw new ArgumentNullException("FileName");
            uint dummy = 0;
            bool dismounted = DeviceIoControl(
                _hFile, // handle to a volume
                EioControlCode.FsctlLockVolume, // dwIoControlCode
                IntPtr.Zero, // lpInBuffer
                0, // nInBufferSize
                IntPtr.Zero, // lpOutBuffer
                0, // nOutBufferSize
                ref dummy,
                IntPtr.Zero // OVERLAPPED structure
            );
        }

        /* ---------------------------------------------------------
         * Move file pointer
         * ------------------------------------------------------ */

        public void moveFilePointer(int cbToMove)
        {
            moveFilePointer(cbToMove, EMoveMethod.FileCurrent);
        }

        public void moveFilePointer(int cbToMove,
            EMoveMethod fMoveMethod)
        {
            if (_hFile != null)
                if (SetFilePointer(_hFile, cbToMove, IntPtr.Zero,
                        fMoveMethod) == InvalidSetFilePointer)
                    throwLastWin32Err();
        }

        public int filePointer
        {
            get
            {
                return (_hFile != null)
                    ? (int) SetFilePointer(_hFile, 0,
                        IntPtr.Zero, EMoveMethod.FileCurrent)
                    : 0;
            }
            set { moveFilePointer(value); }
        }

        /* ---------------------------------------------------------
         * Read and Write
         * ------------------------------------------------------ */

        public uint read(byte[] buffer, uint cbToRead)
        {
            // returns bytes read
            uint cbThatWereRead = 0;
            if (!ReadFile(_hFile, buffer, cbToRead,
                ref cbThatWereRead, IntPtr.Zero))
                throwLastWin32Err();
            return cbThatWereRead;
        }

        public uint write(byte[] buffer, uint cbToWrite)
        {
            // returns bytes read
            uint cbThatWereWritten = 0;
            if (!WriteFile(_hFile, buffer, cbToWrite,
                ref cbThatWereWritten, IntPtr.Zero))
                throwLastWin32Err();
            return cbThatWereWritten;
        }

        /* ---------------------------------------------------------
         * IDisposable Interface
         * ------------------------------------------------------ */
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void dispose(bool fDisposing)
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
            dispose(false);
        }

        public void throwLastWin32Err()
        {
            Marshal.ThrowExceptionForHR(
                Marshal.GetHRForLastWin32Error());
        }
    }
}
