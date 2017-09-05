using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using usbWriteLockTest.data;

namespace usbWriteLockTest.logic
{
    class DeviceCollector
    {


        public List<UsbDevice> volumes = new List<UsbDevice>();

        public DeviceCollector()
        {
            RepollDevices();
        }
        

        public void RepollDevices()
        {
            volumes.Clear();
            List<DriveInfo> drives = DriveInfo.GetDrives().ToList();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                string driveName = (string)queryObj.GetPropertyValue("Name");
                string model = (string) queryObj.GetPropertyValue("Model");
                ulong driveSize = (ulong) queryObj.GetPropertyValue("Size");
                foreach (PropertyData prop in queryObj.Properties)
                {
                    Console.WriteLine("{0}: {1}", prop.Name, prop.Value);
                }
                foreach (ManagementObject b in queryObj.GetRelated("Win32_DiskPartition"))
                {
                    foreach (PropertyData prop in b.Properties)
                    {
                        Console.WriteLine("{0}: {1}", prop.Name, prop.Value);
                    }
                    foreach (ManagementBaseObject c in b.GetRelated("Win32_LogicalDisk"))
                    {
                        var driveInfo = (from n in drives
                            where n.Name.Contains((string)c.GetPropertyValue("DeviceID"))
                            select n).FirstOrDefault();
                        if (driveInfo != null)
                        {
                            UsbDevice device = new UsbDevice(
                                driveName,
                                driveSize,
                                (string)c.GetPropertyValue("DeviceID"),
                                driveInfo,
                                GetDeviceName((string)c.GetPropertyValue("DeviceID"))
                            );
                            volumes.Add(device);
                        }
                    }
                }
            }
        }

        private string GetDeviceName(string deviceID)
        {
            string[] Parts = deviceID.Split(@"\".ToCharArray());
            if (Parts.Length >= 3)
            {
                string DevType = Parts[0];
                string DeviceInstanceId = Parts[1];
                string DeviceUniqueID = Parts[2];
                string RegPath = @"SYSTEM\CurrentControlSet\Enum\" + DevType + "\\" + DeviceInstanceId + "\\" +
                                 DeviceUniqueID;
                RegistryKey key = Registry.LocalMachine.OpenSubKey(RegPath);
                if (key != null)
                {
                    object result = key.GetValue("FriendlyName");
                    if (result != null)
                        return result.ToString();

                }
            }
            return String.Empty;
        }
    }

    
}
