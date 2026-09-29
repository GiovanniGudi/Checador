Constructora AG WPF V10 - Asistencia dos checadores

Correcciones:
- Asistencia consulta Checador Principal y ChecadorSec al mismo tiempo.
- Empleados sigue siendo la tabla administrativa principal.
- Employee/hr_employee de checadores no se modifica.
- Se muestran empleados aunque no tengan checadas.
- Nueva columna: Estado del empleado.
- Nuevo filtro: Todos, Activos, Inactivos.
- Nueva columna: Origen del registro:
  * PRIMARIO
  * SECUNDARIO
  * PRIMARIO Y SECUNDARIO
  * SIN REGISTRO

Pasos:
1. Ejecuta SQL/Patch_V10_Asistencia_DosChecadores.sql en Checador.
2. Abre ConstructoraAG_WPF_Listo.sln.
3. dotnet restore
4. dotnet build
5. Ejecuta.
