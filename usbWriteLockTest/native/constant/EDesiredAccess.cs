using System;

namespace usbWriteLockTest.native.constant
{
    [Flags]
    public enum EDesiredAccess : uint
    {
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000
    }
}
