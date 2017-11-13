using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usbWriteLockTest.logic
{
    enum ApplicationState
    {
        PrepareForTests,
        WriteProtectionEnabled,
        FirstChecksum,
        FirstHashCalculation,
        RunTests,
        SecondChecksum,
        SecondHashCalculation,
        WriteProtectionDisabled,
        CleanupDevice
    }
}
