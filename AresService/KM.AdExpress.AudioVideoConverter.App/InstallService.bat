@ECHO OFF

REM The following directory is for .NET 4
set DOTNETFX4=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX4%

echo Installing MyService...
echo ---------------------------------------------------
InstallUtil /i KM.AdExpress.AudioVideoConverter.App.exe
echo ---------------------------------------------------
echo Done.
pause