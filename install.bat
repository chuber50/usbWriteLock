fltmc unload usbwl
xcopy .\filter\x64\debug\usbwl.sys .\usbWriteLock\ /y
xcopy .\x64\Debug\wlControlApp.exe .\usbWriteLock\ /y
rundll32 syssetup,SetupInfObjectInstallAction DefaultInstall 128 .\usbwl.inf
c:\windows\fltmgr\wlcontrolapp.exe
pause