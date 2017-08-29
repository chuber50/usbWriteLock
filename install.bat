fltmc unload usbwl
xcopy \\wshuber\usbWriteLock\filter\x64\debug\usbwl.sys \\wshuber\usbWriteLock\ /y
xcopy \\wshuber\usbWriteLock\x64\Debug\wlControlApp.exe \\wshuber\usbWriteLock\ /y
REM rundll32 syssetup,SetupInfObjectInstallAction DefaultInstall 128 \\wshuber\usbWriteLock\minispy.inf
REM c:\windows\fltmgr\wlcontrolapp.exe