Constructora AG WPF V7 - Nómina compatible con tu base actual

Corrección:
Tu tabla dbo.NominaPeriodos ya existe con columnas:
- id_periodo
- descripcion
- fecha_inicio
- fecha_fin
- estado
- fecha_creacion

Esta versión adapta el proyecto y el SQL a esa estructura.

Pasos:
1. Ejecuta SQL/Patch_Nomina_V7_Compatible.sql en la base Checador.
2. Abre ConstructoraAG_WPF_Listo.sln.
3. Ejecuta:
   dotnet restore
   dotnet build
4. Abre el módulo Nómina.

Si es instalación nueva también puedes ejecutar:
SQL/ConstructoraAG_SQL.sql
