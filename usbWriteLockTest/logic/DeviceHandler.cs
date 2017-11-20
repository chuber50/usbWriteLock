using  usbWriteLockTest.data;

namespace usbWriteLockTest.logic
{
    class DeviceHandler
    {
        private readonly UsbDrive _usbDrive;

        public DeviceHandler(UsbDrive usbDrive)
        {
            this._usbDrive = usbDrive;
        }

        // try to lock all volumes of the drive
        public void LockVolumes()
        {
            _usbDrive.volumes.ForEach(vol => vol.Lock());
        }

        // try to unlock all volumes of the drive
        public void UnlockVolumes()
        {
            _usbDrive.volumes.ForEach(vol => vol.Unlock());
        }

        // dismount all volumes of the drive
        public void DismountVolumes()
        {
            _usbDrive.volumes.ForEach(vol => vol.Dismount());
        }
    }
}
