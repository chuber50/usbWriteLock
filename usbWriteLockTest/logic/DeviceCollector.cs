using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            drives.Clear();
            List<DriveInfo> driveInfos = DriveInfo.GetDrives().ToList();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");

            foreach (var o in searcher.Get())
            {
                var queryObj = (ManagementObject) o;
                UsbDrive usbDrive = new UsbDrive(
                    (string) queryObj.GetPropertyValue("Name"),
                    (string) queryObj.GetPropertyValue("Model"),
                    (ulong) queryObj.GetPropertyValue("Size"))
                {
                    driveName = (string)queryObj.GetPropertyValue("Name"),
                    model = (string)queryObj.GetPropertyValue("Model"),
                    driveSize = (ulong)queryObj.GetPropertyValue("Size"),
                    sectorsPerTrack = (uint)queryObj.GetPropertyValue("SectorsPerTrack"),
                    totalCylinders = (ulong)queryObj.GetPropertyValue("TotalCylinders"),
                    totalHeads = (uint)queryObj.GetPropertyValue("TotalHeads"),
                    totalSectors = (ulong)queryObj.GetPropertyValue("TotalSectors"),
                    totalTracks = (ulong)queryObj.GetPropertyValue("TotalTracks"),
                    tracksPerCylinder = (uint)queryObj.GetPropertyValue("TracksPerCylinder"),
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
                                name = volumeInfo.Name
                            };

                            usbDrive.volumes.Add(volume);
                        }
                    }
                }
                drives.Add(usbDrive);
            }
        }
    }  
}
