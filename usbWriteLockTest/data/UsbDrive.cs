using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace usbWriteLockTest.data
{
    class UsbDrive
    {
        public List<LogicalVolume> volumes = new List<LogicalVolume>();

        public UsbDrive(string driveName, string model, ulong driveSize)
        {
            this.driveSize = driveSize;
            this.driveName = driveName;
            this.model = model;
        }

        [DisplayName("Drive Name")]
        public string driveName { get; set; }

        [DisplayName("Model")]
        public string model { get; set; }

        [DisplayName("Total Size")]
        public ulong driveSize { get; set; }

        [DisplayName("Bytes/Sector")]
        public uint bytesPerSector { get; set; }

        [DisplayName("Sectors/Track")]
        public uint sectorsPerTrack { get; set; }

        [DisplayName("Total Cylinders")]
        public ulong totalCylinders { get; set; }

        [DisplayName("Total Heads")]
        public uint totalHeads { get; set; }

        [DisplayName("Total Sectors")]
        public ulong totalSectors { get; set; }

        [DisplayName("Total Tracks")]
        public ulong totalTracks { get; set; }

        [DisplayName("Tracks/Cylinder")]
        public uint tracksPerCylinder { get; set; }
    }
}
