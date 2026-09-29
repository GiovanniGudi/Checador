CORRECCION DE COMPILACION - 24/09/2026

Problema detectado:
El archivo .csproj está dentro de PSSF/PSSF, pero varios archivos fuente más recientes (Models, Services y Views) estaban un nivel arriba, en PSSF/. Por eso Visual Studio compilaba un proyecto incompleto y aparecían errores CS0246 por tipos como DestinatarioCorreo y vistas/servicios que MainWindow utiliza. Al fallar la compilación, no se generaba bin/Debug/net8.0-windows/ConstructoraAG.exe y Visual Studio mostraba que el ejecutable no existía.

Corrección:
- Se integraron al directorio real del proyecto PSSF/PSSF todos los Models, Services y Views faltantes.
- Se conservó la corrección de selección de BD administrativa.
- Se eliminaron bin y obj para forzar una reconstrucción limpia.

En Visual Studio:
1. Abrir PSSF/PSSF/ConstructoraAG_WPF_Listo.sln.
2. Compilación > Limpiar solución.
3. Compilación > Recompilar solución.
4. Ejecutar con F5.
