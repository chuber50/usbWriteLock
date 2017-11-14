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

        public void LockVolumes()
        {
            //_usbDrive.volumes.ForEach(vol => vol.Lock());
        }

        public void UnlockVolumes()
        {
            //_usbDrive.volumes.ForEach(vol => vol.Unlock());
        }

        public void DismountVolumes()
        {
            _usbDrive.volumes.ForEach(vol => vol.Dismount());
        }
    }
}
