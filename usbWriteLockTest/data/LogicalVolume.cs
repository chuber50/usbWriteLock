using System;
using System.Collections.Generic;
using System.ComponentModel;
using usbWriteLockTest.native;

namespace usbWriteLockTest.data
{
    class LogicalVolume
    {
        WinApiVolume nativeVolume;

        public LogicalVolume(string deviceId)
        {
            this.deviceId = deviceId;
            nativeVolume = new WinApiVolume(string.Format("\\\\.\\{0}", deviceId));
        }

        [DisplayName("Drive Letter")]
        public System.IO.DirectoryInfo rootDirectory { get; set; }

        [Browsable(false)]
        public string name { get; set; }

        [Browsable(false)]
        public string deviceId { get; private set; }

        [DisplayName("Volume Label")]
        public string volumeLabel { get; set; }

        [DisplayName("Total Size")]
        public long totalSize { get; set; }

        [Browsable(false)]
        public long totalFreeSpace { get; set; }

        [Browsable(false)]
        public bool isReady { get; set; }

        [Browsable(false)]
        public System.IO.DriveType driveType { get; set; }

        [DisplayName("File System")]
        public string driveFormat { get; set; }

        [Browsable(false)]
        public long availableFreeSpace { get; }

        [DisplayName("Exclusive Lock")]
        public bool locked { get; set; } = false;

        [DisplayName("Mounted")]
        public bool mounted { get; set; } = true;

        public void Lock()
        {
            if (!locked)
            {
                locked = nativeVolume.Lock();
            }
        }

        public void Unlock()
        {
            if (locked)
            {
                locked = nativeVolume.Unlock();
            }
        }
    }
}
