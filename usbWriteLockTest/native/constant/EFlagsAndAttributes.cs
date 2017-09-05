using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usbWriteLockTest.native.constant
{
    [Flags]
    public enum EFlagsAndAttributes : uint
    {
        FILE_ATTRIBUTES_ARCHIVE = 0x20,
        FILE_ATTRIBUTE_HIDDEN = 0x2,
        FILE_ATTRIBUTE_NORMAL = 0x80,
        FILE_ATTRIBUTE_OFFLINE = 0x1000,
        FILE_ATTRIBUTE_READONLY = 0x1,
        FILE_ATTRIBUTE_SYSTEM = 0x4,
        FILE_ATTRIBUTE_TEMPORARY = 0x100,
        FILE_FLAG_WRITE_THROUGH = 0x80000000,
        FILE_FLAG_OVERLAPPED = 0x40000000,
        FILE_FLAG_NO_BUFFERING = 0x20000000,
        FILE_FLAG_RANDOM_ACCESS = 0x10000000,
        FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000,
        FILE_FLAG_DELETE_ON = 0x4000000,
        FILE_FLAG_POSIX_SEMANTICS = 0x1000000,
        FILE_FLAG_OPEN_REPARSE_POINT = 0x200000,
        FILE_FLAG_OPEN_NO_CALL = 0x100000
    }
}
