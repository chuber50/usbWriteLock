using System;

namespace usbWriteLockTest.native.constant
{
    [Flags]
    public enum EFlagsAndAttributes : uint
    {
        FileAttributesArchive = 0x20,
        FileAttributeHidden = 0x2,
        FileAttributeNormal = 0x80,
        FileAttributeOffline = 0x1000,
        FileAttributeReadonly = 0x1,
        FileAttributeSystem = 0x4,
        FileAttributeTemporary = 0x100,
        FileFlagWriteThrough = 0x80000000,
        FileFlagOverlapped = 0x40000000,
        FileFlagNoBuffering = 0x20000000,
        FileFlagRandomAccess = 0x10000000,
        FileFlagSequentialScan = 0x8000000,
        FileFlagDeleteOn = 0x4000000,
        FileFlagPosixSemantics = 0x1000000,
        FileFlagOpenReparsePoint = 0x200000,
        FileFlagOpenNoCall = 0x100000
    }
}
