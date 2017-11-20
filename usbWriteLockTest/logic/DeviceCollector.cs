using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;

using usbWriteLockTest.data;

namespace usbWriteLockTest.logic
{
    class DeviceCollector
    {
        public List<UsbDrive> drives = new List<UsbDrive>();

        public DeviceCollector()
        {
            repollDevices();
        }

        // queries the WMI for devices and populates the drives list
        public void repollDevices()
        {
            drives.ForEach(d => { d.upToDate = false; });
            drives.ForEach(d => d.volumes.ForEach(v => { v.isUpToDate = false; }));

            List<DriveInfo> driveInfos = DriveInfo.GetDrives().ToList();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");

            foreach (var o in searcher.Get())
            {
                int index;
                bool volumesHaveChanged = false;
                var queryObj = (ManagementObject) o;
                UsbDrive newDrive = new UsbDrive(
                    (string) queryObj.GetPropertyValue("Name"),
                    (string) queryObj.GetPropertyValue("Model"),
                    (ulong) queryObj.GetPropertyValue("Size"))
                {
                    driveName = (string)queryObj.GetPropertyValue("Name"),
                    model = (string)queryObj.GetPropertyValue("Model"),
                    driveSize = (ulong)queryObj.GetPropertyValue("Size"),
                    serialNo = (string)queryObj.GetPropertyValue("SerialNumber"),
                    sectorsPerTrack = (uint)queryObj.GetPropertyValue("SectorsPerTrack"),
                    totalCylinders = (ulong)queryObj.GetPropertyValue("TotalCylinders"),
                    totalHeads = (uint)queryObj.GetPropertyValue("TotalHeads"),
                    totalSectors = (ulong)queryObj.GetPropertyValue("TotalSectors"),
                    totalTracks = (ulong)queryObj.GetPropertyValue("TotalTracks"),
                    tracksPerCylinder = (uint)queryObj.GetPropertyValue("TracksPerCylinder"),
                    bytesPerSector = (uint)queryObj.GetPropertyValue("BytesPerSector"),
                    upToDate = true
                };

                //foreach (PropertyData prop in queryObj.Properties)
                //{
                //    Console.WriteLine("{0}: {1}", prop.Name, prop.Value);
                //}

                foreach (var managementBaseObject in queryObj.GetRelated("Win32_DiskPartition"))
                {
                    var b = (ManagementObject) managementBaseObject;
                    foreach (ManagementBaseObject c in b.GetRelated("Win32_LogicalDisk"))
                    {
                        var volumeInfo = (from n in driveInfos
                            where n.Name.Contains((string)c.GetPropertyValue("DeviceID"))
                            select n).FirstOrDefault();
                        if (volumeInfo != null)
                        {
                            LogicalVolume volume =
                                new LogicalVolume((string) c.GetPropertyValue("DeviceID"))
                                {
                                    driveFormat = volumeInfo.DriveFormat,
                                    rootDirectory = volumeInfo.RootDirectory,
                                    driveType = volumeInfo.DriveType,
                                    totalFreeSpace = volumeInfo.TotalFreeSpace,
                                    totalSize = volumeInfo.TotalSize,
                                    isReady = volumeInfo.IsReady,
                                    volumeLabel = volumeInfo.VolumeLabel,
                                    name = volumeInfo.Name,
                                    isUpToDate = true
                                };

                            if ((index = newDrive.volumes.IndexOf(volume)) != -1)
                            {
                                newDrive.volumes[index].isUpToDate = true;
                            }
                            else
                            {
                                newDrive.volumes.Add(volume);
                                volumesHaveChanged = true;
                            } 

                        }
                    }
                }

                if (newDrive.volumes.RemoveAll(v => !v.isUpToDate) > 0)
                {
                    volumesHaveChanged = true;
                } 

                if ((index = drives.IndexOf(newDrive)) != -1)
                {
                    drives[index].upToDate = true;
                    if (volumesHaveChanged)
                    {
                        drives[index].volumes = newDrive.volumes;
                    }
                }
                else
                {
                    drives.Add(newDrive);
                }
            }

            drives.RemoveAll(d => !d.upToDate);
        }

        public void ClearHashes()
        {
            foreach (UsbDrive drive in drives)
            {
                drive.firstHash = null;
                drive.secondHash = null;
            }
        }
    }  
}
