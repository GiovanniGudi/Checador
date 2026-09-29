Constructora AG - V5 Registrado en Checadores

Corrección principal:
- El módulo Empleados ahora consulta dbo.vw_empleados_consulta.
- Muestra claramente:
  * ID Checador Principal
  * ID Checador Secundario
  * Registrado en

Valores de "Registrado en":
- Solo Checador Principal
- Solo Checador Secundario
- Ambos Checadores
- Sin registro en Checadores

También se mantiene:
- Origen del registro en asistencias.
- Estado Activo/Inactivo.
- Filtros y módulos V4.

Pasos:
1. Ejecuta SQL/ConstructoraAG_SQL.sql en Checador.
2. Abre ConstructoraAG_WPF_Listo.sln.
3. dotnet restore
4. dotnet build
5. Ejecuta.
