using System.Collections.Generic;
using System.ComponentModel;
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
        

        public void repollDevices()
        {
            foreach (UsbDrive drive in drives)
            {
                drive.upToDate = false;
                foreach (LogicalVolume vol in drive.volumes)
                {
                    vol.isUpToDate = false;
                }
            }

            //BindingList does not support
            //drives.ForEach(d => { d.upToDate = false; });
            //drives.ForEach(d => d.volumes.ForEach(v => { v.isUpToDate = false; }));

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
                            LogicalVolume volume = new LogicalVolume((string)c.GetPropertyValue("DeviceID"))
                            {
                                rootDirectory = volumeInfo.RootDirectory,
                                driveFormat =  volumeInfo.DriveFormat,
                                driveType = volumeInfo.DriveType,
                                isReady = volumeInfo.IsReady,
                                totalFreeSpace = volumeInfo.TotalFreeSpace,
                                totalSize = volumeInfo.TotalSize,
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

            foreach(UsbDrive drive in drives)
            {
                if (!drive.upToDate)
                {
                    //https://stackoverflow.com/questions/142003/cross-thread-operation-not-valid-control-accessed-from-a-thread-other-than-the
                    drives.Remove(drive);
                }
            }
            //drives.RemoveAll(d => !d.upToDate);
        }

        public void ClearHashes()
        {
            foreach (UsbDrive drive in drives)
            {
                drive.hashes.Clear();
            }
        }
    }  
}
