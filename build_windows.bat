@echo off
rem One-step build for Windows.
rem   build_windows.bat path\to\StrangerThings.apkm   first time: prepare assets from your APK, then build
rem   build_windows.bat                               later: rebuild (asks for the APK if setup never ran)
rem Extra options (--dev, --clean, --full-clean) are passed on to tools\build.py.
setlocal
cd /d "%~dp0"

set "PY="
for %%c in ("py -3" python python3) do (
    if not defined PY (
        %%~c -c "import sys; sys.exit(sys.version_info < (3, 10))" >nul 2>&1 && set "PY=%%~c"
    )
)
if not defined PY (
    echo ERROR: Python 3.10 or newer is required. Install it from https://www.python.org/downloads/ and try again.
    goto :fail
)

set "APK="
set "FIRST=%~1"
if defined FIRST if not "%FIRST:~0,1%"=="-" (
    set "APK=%~1"
    shift
)
if defined APK (
    %PY% tools\setup.py "%APK%" || goto :fail
) else if not exist ".cache\setup_ok.json" (
    %PY% tools\setup.py || goto :fail
)

set "REST="
:collect
if "%~1"=="" goto :build
set "REST=%REST% %1"
shift
goto :collect

:build
%PY% tools\build.py --target windows%REST% || goto :fail
echo.
echo Start the game with:  dist\windows\StrangerThings1984.exe
call :maybe_pause
endlocal
exit /b 0

:fail
echo.
echo Build did not complete. See the message above.
call :maybe_pause
endlocal
exit /b 1

rem Keep the window open when the script was started by double-clicking it in Explorer.
:maybe_pause
echo %CMDCMDLINE% | find /i "/c" >nul && pause
exit /b 0
