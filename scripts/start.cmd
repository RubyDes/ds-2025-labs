@echo off
title Запуск системы PA3 с 3 потребителями

echo Запуск NATS-сервера...
start "" /D "..\nats-server\" nats-server.exe -DV

echo Запуск 3 экземпляров RankCalculator...
start "RankCalculator 1" /D "..\RankCalculator\" dotnet run
start "RankCalculator 2" /D "..\RankCalculator\" dotnet run
start "RankCalculator 3" /D "..\RankCalculator\" dotnet run

echo Запуск Valuator...
start "" /D "..\Valuator\" dotnet run --urls "http://0.0.0.0:5001"

echo Система запущена. Нажмите Enter для выхода...
pause