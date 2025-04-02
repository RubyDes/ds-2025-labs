@echo off
REM 
chcp 65001 >nul

REM Переход в директорию проекта
cd /d "C:\Users\User\Documents\Волгатех ПС - 32\Распределенное программирование\ds-2025\Valuator"

REM
echo Запуск приложения на порту 5001...
start dotnet run --urls "http://0.0.0.0:5001"
if %errorlevel% neq 0 (
    echo Ошибка: Не удалось запустить приложение на порту 5001.
    exit /b 1
)

echo Запуск приложения на порту 5002...
start dotnet run --urls "http://0.0.0.0:5002"
if %errorlevel% neq 0 (
    echo Ошибка: Не удалось запустить приложение на порту 5002.
    exit /b 1
)

REM
echo Запуск Nginx...
cd /d C:\nginx-1.27.4
start nginx
if %errorlevel% neq 0 (
    echo Ошибка: Не удалось запустить Nginx.
    exit /b 1
)

REM
timeout /t 2 >nul
tasklist | findstr "nginx.exe" >nul
if %errorlevel% neq 0 (
    echo Ошибка: Nginx не запущен.
    exit /b 1
)

echo Все компоненты запущены.