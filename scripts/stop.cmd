@echo off
REM Остановка всех процессов dotnet
taskkill /f /im dotnet.exe

REM Остановка Nginx
taskkill /f /im nginx.exe

echo Все компоненты остановлены.