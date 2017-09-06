using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  usbWriteLockTest.data;
using usbWriteLockTest.logic;

using System.IO;
using System.Security.Cryptography;
using usbWriteLockTest.native;

namespace usbWriteLockTest.logic
{
    class DiskReader
    {
        private readonly UsbDrive _usbDrive;

        public DiskReader(UsbDrive usbDrive)
        {
            this._usbDrive = usbDrive;
        }

        public void LockVolumes()
        {
            _usbDrive.volumes.ForEach(vol => vol.Lock());
        }

        public void UnlockVolumes()
        {
            _usbDrive.volumes.ForEach(vol => vol.Unlock());
        }

        public void GenerateChecksum()
        {
            //https://stackoverflow.com/questions/5805106/hashing-multiple-bytes-together-into-a-single-hash-with-c
            SHA256Managed sha = new SHA256Managed();

            //sha.TransformBlock(Buffer, 0, Buffer.len);
            //byte[] aBuffer = new byte[512];
            //uint cbRead = file.Read(aBuffer, 1000);
            //file.Close();
            string hash;

            using (DiskStream stream = new DiskStream(_usbDrive.driveName, FileAccess.Read, _usbDrive.bytesPerSector, _usbDrive.driveSize))
            {
                
                byte[] checksum = sha.ComputeHash(stream);
                hash =  BitConverter.ToString(checksum).Replace("-", String.Empty);
            }

           
        }
    }
}
