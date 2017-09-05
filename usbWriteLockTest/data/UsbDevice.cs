using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace usbWriteLockTest.data
{
    class UsbDevice
    {
        public UsbDevice(string driveName, ulong driveSize, string deviceId, DriveInfo driveInfo, string friendlyName)
        {
            DriveSize = driveSize;
            DriveName = driveName;
            DeviceID = deviceId;
            VolumeLabel = driveInfo.VolumeLabel;
            TotalSize = driveInfo.TotalSize;
            TotalFreeSpace = driveInfo.TotalFreeSpace;
            RootDirectory = driveInfo.RootDirectory;
            Name = driveInfo.Name;
            IsReady = driveInfo.IsReady;
            DriveType = driveInfo.DriveType;
            DriveFormat = driveInfo.DriveFormat;
            AvailableFreeSpace = driveInfo.AvailableFreeSpace;
            FriendlyName = friendlyName;
        }

        public ulong DriveSize { get; }

        [DisplayName("Drive Name")]
        public string DriveName { get; }

        [DisplayName("Drive Letter")]
        public System.IO.DirectoryInfo RootDirectory { get; }

        [Browsable(false)]
        public string Name { get; }

        [Browsable(false)]
        public string DeviceID { get; private set; }

        [DisplayName("Volume Label")]
        public string VolumeLabel { get; }

        [DisplayName("Total Size")]
        public long TotalSize { get; }

        [Browsable(false)]
        public long TotalFreeSpace { get; }

        [Browsable(false)]
        public bool IsReady { get; }

        [Browsable(false)]
        public System.IO.DriveType DriveType { get; }

        [DisplayName("File System")]
        public string DriveFormat { get; }

        [Browsable(false)]
        public long AvailableFreeSpace { get; }

        [Browsable(false)]
        public string FriendlyName { get; }
    }
}
