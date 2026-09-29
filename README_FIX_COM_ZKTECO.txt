CORRECCION ERROR ZKTECO 0x80040154 / REGDB_E_CLASSNOTREG
========================================================

Problema observado:
Retrieving the COM class factory for component with CLSID
{00853A19-BD51-419B-9269-2DABE57EB61F} failed - 0x80040154.

Causa:
La aplicacion se publicaba como win-x64, pero zkemkeeper es el SDK COM de
32 bits usado por estos checadores. Windows separa el registro COM x86/x64,
por lo que una aplicacion x64 no puede crear una clase COM registrada solo x86.

Cambios realizados:
- Proyecto forzado a PlatformTarget=x86.
- publicar_exe.bat cambiado de win-x64 a win-x86.
- Agregado SDK\\Instalar_SDK_ZK.bat para registrar zkemkeeper.dll con
  SysWOW64\\regsvr32.exe.
- Mensaje de error mejorado cuando falta el registro COM.
- No se cambio la logica de asignacion de bases de datos por checador.

IMPORTANTE:
En la PC final, zkemkeeper.dll debe estar instalada/registrada. El ZIP no
incluye una DLL propietaria del fabricante si esta no venia ya en el proyecto.
