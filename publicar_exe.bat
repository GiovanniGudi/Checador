@echo off
setlocal
cd /d "%~dp0"

echo ================================================
echo  CONSTRUCTORA AG - PUBLICACION WINDOWS x86
echo  Requerido para SDK ZKTeco zkemkeeper 32 bits
echo ================================================

dotnet restore
if errorlevel 1 goto :error

dotnet publish -c Release -r win-x86 --self-contained true -p:PlatformTarget=x86
if errorlevel 1 goto :error

echo.
echo Publicacion terminada correctamente.
echo Revise: bin\Release\net8.0-windows\win-x86\publish
goto :fin

:error
echo.
echo ERROR: no fue posible publicar el proyecto.

:fin
pause
endlocal
