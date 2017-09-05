using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  usbWriteLockTest.data;
using usbWriteLockTest.logic;

using System.IO;
using usbWriteLockTest.logic;

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

            WinApiFile file = new WinApiFile(diskName,
            WinApiFile.DesiredAccess.GENERIC_READ);

            byte[] aBuffer = new byte[512];
            uint cbRead = file.Read(aBuffer, 1000);
            file.Close();
        }
       
    }
}
