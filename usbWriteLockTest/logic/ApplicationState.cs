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
