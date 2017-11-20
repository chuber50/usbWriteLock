using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using usbWriteLockTest.data;
using usbWriteLockTest.native;

namespace usbWriteLockTest.logic
{
    public class AsyncHashCalculator
    {
        private const int BufferSize = 4096;
        private readonly UsbDrive _usbDrive;
        private readonly HashAlgorithm _hashAlgorithm;

        private readonly BackgroundWorker _backgroundWorker;
        public event ProgressChangedEventHandler progressChanged;

        public AsyncHashCalculator(BackgroundWorker backgroundWorker, UsbDrive usbDrive)
        {
            _usbDrive = usbDrive;
            _hashAlgorithm = new SHA256Managed();
            _backgroundWorker = backgroundWorker;
            _backgroundWorker.ProgressChanged += handleProgressChanged;
        }

        // the progres event
        private void handleProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressChanged?.Invoke(this, e);
        }

        // computes a hash by reading the disk in chunks of 4kb
        // reports back its progress to the main process
        public string computeHash()
        {
            long totalBytesRead = 0;

            using (DiskStream stream = new DiskStream(_usbDrive.driveName, FileAccess.Read, _usbDrive.bytesPerSector,
                _usbDrive.driveSize))
            {
                var size = stream.Length;

                var readAheadBuffer = new byte[BufferSize];
                var readAheadBytesRead = stream.Read(readAheadBuffer, 0, readAheadBuffer.Length);

                totalBytesRead += readAheadBytesRead;

                do
                {
                    var bytesRead = readAheadBytesRead;
                    var buffer = readAheadBuffer;

                    readAheadBuffer = new byte[BufferSize];
                    readAheadBytesRead = stream.Read(readAheadBuffer, 0, readAheadBuffer.Length);
                    totalBytesRead += readAheadBytesRead;

                    if (readAheadBytesRead == 0)
                        _hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
                    else
                        _hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);

                    _backgroundWorker.ReportProgress((int) ((double) totalBytesRead * 100 / size));
                } while (readAheadBytesRead != 0 && !_backgroundWorker.CancellationPending);

                if (_backgroundWorker.CancellationPending)
                    return String.Empty;

                _usbDrive.AddHash(BitConverter.ToString(_hashAlgorithm.Hash).Replace("-", string.Empty));
                
                return BitConverter.ToString(_hashAlgorithm.Hash).Replace("-", string.Empty);

                //http://www.alexandre-gomes.com/?p=144
                //http://www.infinitec.de/post/2007/06/09/Displaying-progress-updates-when-hashing-large-files.aspx
            }
        }
    }
}
