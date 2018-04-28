@ECHO off
@TITLE Kotocorn
CD /D "C:\Users\jayd\OneDrive\Development\Kotocorn"
dotnet restore
CD /D "C:\Users\jayd\OneDrive\Development\Kotocorn\src\Nadekobot"
dotnet run
ECHO NadekoBot has been succesfully stopped, press any key to close this window.
TITLE Kotocorn - Stopped
CD /D "%~dp0"
PAUSE >nul 2>&1
del NadekoRun.bat