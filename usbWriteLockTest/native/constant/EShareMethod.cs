using System;

namespace usbWriteLockTest.native.constant
{
    [Flags]
    public enum EShareMode : uint
    {
        FileShareNone = 0x0,
        FileShareRead = 0x1,
        FileShareWrite = 0x2,
        FileShareDelete = 0x4,

    }
}
