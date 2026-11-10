@echo off
setlocal
chcp 65001 >nul
cd /d "%~dp0"
set "DOTNET=dotnet"
if exist "%~dp0.dotnet\dotnet.exe" set "DOTNET=%~dp0.dotnet\dotnet.exe"
set "TFM=net9.0-windows10.0.19041.0"
set "APP=%~dp0MauiApp3\bin\Debug\%TFM%\win10-x64\MauiApp3.exe"

"%DOTNET%" build -f "%TFM%" "%~dp0MauiApp3\MauiApp3.csproj"
if errorlevel 1 goto :FAILED
if not exist "%APP%" goto :FAILED
start "" "%APP%"
exit /b 0

:FAILED
echo.
echo [BLAD] Nie udalo sie uruchomic aplikacji.
pause
exit /b 1
