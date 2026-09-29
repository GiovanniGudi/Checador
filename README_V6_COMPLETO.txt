Constructora AG WPF V6 Completo

Incluye:
- Login local independiente de SQL Server.
- Dashboard.
- Empleados con ID Checador Principal, ID Checador Secundario y Registrado en.
- Asistencias con Origen del Registro.
- Incidencias.
- Configuración General.
- Catálogos.
- Nómina formal congelada.
- Tablas NominaPeriodos y NominaDetalleFormal incluidas.
- Bitácora.

Instalación:
1. Ejecuta SQL/ConstructoraAG_SQL.sql en la base Checador.
2. Abre ConstructoraAG_WPF_Listo.sln en Visual Studio.
3. Ejecuta:
   dotnet restore
   dotnet build
4. Inicia sesión con:
   admin / admin

Si ya habías ejecutado SQL anterior y solo te falla Nómina:
Ejecuta SQL/Patch_Nomina_V6.sql
