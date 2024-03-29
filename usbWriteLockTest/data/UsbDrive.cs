﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace usbWriteLockTest.data
{
    public class UsbDrive : IEquatable<UsbDrive>
    {
        public List<LogicalVolume> volumes = new List<LogicalVolume>();
        public TestMeta testMeta;

        public UsbDrive(string driveName, string model, ulong driveSize)
        {
            this.driveSize = driveSize;
            this.driveName = driveName;
            this.model = model;
        }

        public void AddHash(string hashCode)
        {
            if (firstHash == null)
            {
                firstHash = hashCode;
                
            }
            else
            {
                secondHash = hashCode;
            }
        }

        [DisplayName("Drive Name")]
        public string driveName { get; set; }

        [DisplayName("Model")]
        public string model { get; set; }

        [DisplayName("Total Size")]
        public ulong driveSize { get; set; }

        [DisplayName("Serial No.")]
        public string serialNo { get; set; }

        [Browsable(false)]
        public uint bytesPerSector { get; set; }

        [Browsable(false)]
        public uint sectorsPerTrack { get; set; }

        [Browsable(false)]
        public ulong totalCylinders { get; set; }

        [Browsable(false)]
        public uint totalHeads { get; set; }

        [Browsable(false)]
        public ulong totalSectors { get; set; }

        [Browsable(false)]
        public ulong totalTracks { get; set; }

        [Browsable(false)]
        public uint tracksPerCylinder { get; set; }

        [Browsable(false)]
        public bool upToDate { get; set; }

        [Browsable(false)]
        public string firstHash { get; set; }

        [Browsable(false)]
        public string secondHash { get; set; }

        public bool Equals(UsbDrive other)
        {
            return String.Compare(driveName, other.driveName, StringComparison.Ordinal) == 0 &&
                   String.Compare(model, other.model, StringComparison.Ordinal) == 0 &&
                   driveSize == other.driveSize;
        }

        public TestMeta getTestVolume()
        {
            if (testMeta != null) return testMeta;

            if (volumes.Count > 0)
            {
                testMeta = new TestMeta(volumes.FirstOrDefault(v => v.totalFreeSpace > TestMeta.CMinfreespace));
                return testMeta;
            }

            return null;
        }

        public bool hashesAreEqual()
        {
            return firstHash.Equals(secondHash);
        }
    }
}
