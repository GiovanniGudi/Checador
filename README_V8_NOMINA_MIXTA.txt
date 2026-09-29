Constructora AG WPF V8 - Nómina estructura mixta

Esta versión corrige el caso real detectado:

dbo.NominaPeriodos:
- id_periodo
- descripcion
- fecha_inicio
- fecha_fin
- estado
- tipo_periodo

dbo.NominaDetalleFormal:
- IdPeriodo
- TotalNeto
- IdDetalle
- IdEmpleado
- NombreCompleto
- etc.

Pasos:
1. Ejecuta SQL/Patch_Nomina_V8_Mixta.sql en Checador.
2. Abre ConstructoraAG_WPF_Listo.sln.
3. dotnet restore
4. dotnet build
5. Prueba el módulo Nómina.
