@ECHO off
@TITLE Kotocorn
CD /D "C:\Users\jayd\OneDrive\Development\Kotocorn"
dotnet restore
dotnet build --configuration Release
CD /D "C:\Users\jayd\OneDrive\Development\Kotocorn\src\NadekoBot"
dotnet run
ECHO NadekoBot has been succesfully stopped, press any key to close this window.
TITLE NadekoBot - Stopped
CD /D "%~dp0"
PAUSE >nul 2>&1
del NadekoRun.bat