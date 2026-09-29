Constructora AG WPF - Versión mejorada

Incluye:
- Login local independiente de SQL Server.
- Dashboard más visual con tarjetas e indicadores.
- Empleados con ID Checador Principal e ID Checador Secundario.
- Días de vacaciones en lugar de saldo de vacaciones.
- Tipo de pago: Semanal, Quincenal, Mensual.
- Descuentos por falta, retardo y horario incompleto.
- Asistencias con columna "Origen del registro".
- Incidencias con crear, editar, borrar, justificar y cancelar justificación.
- Filtros de incidencias por estado, fechas, tipo y checador.
- Leyenda: Constructora AG.

Pasos:
1. Ejecuta SQL/ConstructoraAG_SQL.sql en la base Checador.
2. Abre ConstructoraAG_WPF_Listo.sln en Visual Studio.
3. Ejecuta dotnet restore.
4. Ejecuta dotnet build.
5. Ejecuta el sistema.
6. Login local: admin / admin.
7. Configura SQL desde el módulo Conexiones BD.

Nota:
La tabla principal es dbo.Empleados.
Checador y ChecadorSec solo son fuentes externas de registros.
