﻿using System;
using System.ComponentModel;
using usbWriteLockTest.native;

namespace usbWriteLockTest.data
{
    public class LogicalVolume : IEquatable<LogicalVolume>
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

        [Browsable(false)]
        public bool isUpToDate { get; set; }

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

        public void Dismount()
        {
            if (mounted)
            {
                mounted = nativeVolume.Dismount();
            }
        }

        public bool Equals(LogicalVolume other)
        {
            return String.Compare(deviceId, other.deviceId, StringComparison.Ordinal) == 0 &&
                   totalSize == other.totalSize;
        }
    }
}
