@echo off
title NSBM Industry Collaboration Unit - Startup
cd /d "%~dp0"

echo.
echo  ==============================================================
echo    NSBM GREEN UNIVERSITY  ^|  Industry Collaboration Unit
echo  ==============================================================
echo.

:: 1. Web dependencies (first run only)
if not exist "web\node_modules" (
  echo [1/3] Installing web dependencies...
  call npm install --prefix web --no-audit --no-fund
  if errorlevel 1 ( echo ERROR: npm install failed. & pause & exit /b 1 )
) else (
  echo [1/3] Web dependencies already installed.
)
echo.

:: 2. API (applies migrations and seeds demo data on start-up)
echo [2/3] Starting API on http://localhost:5300 ...
start "NSBM ICU API" /min cmd /c "cd /d "%~dp0src\NSBM.ICU.API" && dotnet run --launch-profile http"
timeout /t 5 /nobreak >nul
echo.

:: 3. Web client
echo [3/3] Starting web client on http://localhost:5301 ...
start "NSBM ICU Web" /min cmd /c "cd /d "%~dp0web" && npm run dev"
timeout /t 5 /nobreak >nul

start "" "http://localhost:5301"
echo.
echo  ==============================================================
echo    API      : http://localhost:5300   (Swagger: /swagger)
echo    Web      : http://localhost:5301
echo.
echo    Demo accounts (fictional data):
echo      Admin    : icu.head@nsbm.demo   /  Admin@123
echo      Lecturer : lecturer@nsbm.demo   /  Lecturer@123
echo      Student  : student@nsbm.demo    /  Student@123
echo      Industry : partner@nsbm.demo    /  Industry@123   (also partner2@, partner3@)
echo  ==============================================================
echo.
echo  To stop: close the "NSBM ICU API" and "NSBM ICU Web" windows.
pause
