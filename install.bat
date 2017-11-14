fltmc unload usbwl
xcopy \\WSHUBER\usbWriteLock\filter\x64\debug\usbwl.sys \\WSHUBER\usbWriteLock\usbwl.sys /y
xcopy \\WSHUBER\usbWriteLock\x64\Debug\wlControlApp.exe \\WSHUBER\usbWriteLock\wlControlApp.exe /y
rundll32 syssetup,SetupInfObjectInstallAction DefaultInstall 128 \\WSHUBER\usbWriteLock\usbwl.inf
c:\windows\fltmgr\wlcontrolapp.exe
pause