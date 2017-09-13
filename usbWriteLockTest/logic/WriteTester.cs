using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using usbWriteLockTest.data;

namespace usbWriteLockTest.logic
{
    class WriteTester
    {
        private const int CMinfreespace = 3000000;
        private const string CMsgSuccess = "Success";

        public WriteTester(UsbDrive usbDrive)
        {
            var volume = getSuitableVolume(usbDrive);
            if (volume != null)
            {
                volumePath = volume.name;
                dirName = Path.Combine(volumePath, generateUniqueId());
                fileName = Path.Combine(volumePath, generateUniqueId());
                hasValidVolume = true;
            }
        }

        private string volumePath { get; set; }
        private string dirName { get; }
        private string fileName { get; }
        public bool hasValidVolume { get; }

        public string test1_CreateFolder()
        {
            string msg = CMsgSuccess;
            try
            {
                Directory.CreateDirectory(dirName);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to create folder {dirName}: {msg}";
        }

        //TODO create before lock?
        public string test2_DeleteFolder()
        {
            string msg = CMsgSuccess;
            try
            {
                Directory.Delete(dirName);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to delete folder {dirName}: {msg}";
        }

        public string test3_CreateFile()
        {
            string msg = CMsgSuccess;
            try
            {
                Directory.Delete(dirName);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return $"Trying to delete folder {dirName}: {msg}";
        }

        private string generateUniqueId()
        {
            return "WLTMP_" + Guid.NewGuid().ToString("D");
        }

        private LogicalVolume getSuitableVolume(UsbDrive usbDrive)
        {
            if (usbDrive.volumes.Count > 0)
            {
                return usbDrive.volumes.FirstOrDefault(v => v.totalFreeSpace > CMinfreespace);
            }

            return null;
        }
    }
}
