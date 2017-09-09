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
    class DeviceHandler
    {
        private readonly UsbDrive _usbDrive;

        public DeviceHandler(UsbDrive usbDrive)
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

        public void DismountVolumes()
        {
            _usbDrive.volumes.ForEach(vol => vol.Dismount());
        }

        public void GenerateChecksum()
        {
            //https://stackoverflow.com/questions/5805106/hashing-multiple-bytes-together-into-a-single-hash-with-c
            //https://stackoverflow.com/questions/1177607/what-is-the-fastest-way-to-create-a-checksum-for-large-files-in-c-sharp
            SHA256Managed sha = new SHA256Managed();

            string hash;

            using (DiskStream stream = new DiskStream(_usbDrive.driveName, FileAccess.Read, _usbDrive.bytesPerSector, _usbDrive.driveSize))
            {
                
                byte[] checksum = sha.ComputeHash(stream);
                hash =  BitConverter.ToString(checksum).Replace("-", String.Empty);
            }

            _usbDrive.computedHash = hash;
        }
    }
}
