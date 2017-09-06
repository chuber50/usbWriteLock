using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  usbWriteLockTest.data;
using usbWriteLockTest.logic;

using System.IO;

namespace usbWriteLockTest.data
{
    class DiskReader
    {
        // IntPtr hDrive = CreateFile(
        //     string.Format("\\\\.\\{0}:", "A"),
        // (uint)NativeMethods.DesiredAccess.GenericRead,
        // (uint)FileShare.None, // exclusice access
        // IntPtr.Zero,
        //(uint) CreationDisposition.OpenExisting,
        // 0,
        // IntPtr.Zero);

        public DiskReader(string diskName)
        {
            WinApiVolume vol = new WinApiVolume(string.Format("\\\\.\\{0}:", "F"));
            vol.Lock();

            vol.unLock();

            vol.close();

            //byte[] aBuffer = new byte[512];
            //uint cbRead = file.Read(aBuffer, 1000);
            //file.Close();
        }
       
    }
}
