using System.Collections.Generic;
using System.ComponentModel;

namespace usbWriteLockTest.data
{
    public class UsbDrive
    {
        public List<LogicalVolume> volumes = new List<LogicalVolume>();
        public List<Hash> hashes = new List<Hash>();

        public UsbDrive(string driveName, string model, ulong driveSize)
        {
            this.driveSize = driveSize;
            this.driveName = driveName;
            this.model = model;
        }

        public void AddHash(string hashCode)
        {
            hashes.Add(new Hash(hashes.Count + 1, hashCode));
        }

        [DisplayName("Drive Name")]
        public string driveName { get; set; }

        [DisplayName("Model")]
        public string model { get; set; }

        [DisplayName("Total Size")]
        public ulong driveSize { get; set; }

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

    }
}
