using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using usbWriteLockTest.data;
using usbWriteLockTest.native;

namespace usbWriteLockTest.logic
{
    public class AsyncHashCalculator
    {
        private readonly int _bufferSize = 4096;
        private readonly UsbDrive _usbDrive;
        private HashAlgorithm _hashAlgorithm;
        protected byte[] hash;

        private BackgroundWorker _backgroundWorker;
        public event ProgressChangedEventHandler progressChanged;

        public AsyncHashCalculator(BackgroundWorker backgroundWorker, UsbDrive usbDrive)
        {
            _usbDrive = usbDrive;
            _hashAlgorithm = new SHA256Managed();
            _backgroundWorker = backgroundWorker;
            _backgroundWorker.ProgressChanged += handleProgressChanged;
        }

        private void handleProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressChanged?.Invoke(this, e);
        }

        public byte[] ComputeHash()
        {
            Debug.Assert(_usbDrive.volumes.TrueForAll(v => v.locked));

            long totalBytesRead = 0;

            using (DiskStream stream = new DiskStream(_usbDrive.driveName, FileAccess.Read, _usbDrive.bytesPerSector,
                _usbDrive.driveSize))
            {
                var size = stream.Length;

                var readAheadBuffer = new byte[_bufferSize];
                var readAheadBytesRead = stream.Read(readAheadBuffer, 0, readAheadBuffer.Length);

                totalBytesRead += readAheadBytesRead;

                do
                {
                    var bytesRead = readAheadBytesRead;
                    var buffer = readAheadBuffer;

                    readAheadBuffer = new byte[_bufferSize];
                    readAheadBytesRead = stream.Read(readAheadBuffer, 0, readAheadBuffer.Length);
                    totalBytesRead += readAheadBytesRead;

                    if (readAheadBytesRead == 0)
                        _hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
                    else
                        _hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);

                    _backgroundWorker.ReportProgress((int) ((double) totalBytesRead * 100 / size));
                } while (readAheadBytesRead != 0 && !_backgroundWorker.CancellationPending);

                if (_backgroundWorker.CancellationPending)
                    return hash = null;

                return hash = _hashAlgorithm.Hash;

                //TODO make cancellable
                //http://www.alexandre-gomes.com/?p=144
                //http://www.infinitec.de/post/2007/06/09/Displaying-progress-updates-when-hashing-large-files.aspx
            }
        }
    }
}
