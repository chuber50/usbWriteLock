using System;

namespace usbWriteLockTest.native
{
    [Flags]
    public enum EMethod : uint
    {
        Buffered = 0,
        InDirect = 1,
        OutDirect = 2,
        Neither = 3
    }
}
