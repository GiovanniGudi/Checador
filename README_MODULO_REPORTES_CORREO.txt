Módulo agregado: Configuración y Envío de Reportes por Correo Electrónico

Incluye:
- Configuración SMTP: servidor, puerto, correo, contraseña/token, SSL/TLS y nombre del remitente.
- Botón Prueba de Conexión que envía correo de prueba a la misma cuenta remitente.
- Administración de hasta 10 destinatarios: agregar, editar, eliminar y activar/pausar.
- Selección combinable de reportes: asistencias, asistencias con entrada/salida, justificaciones, empleados y dashboard.
- Envío inmediato con asunto y cuerpo configurables, usando {fecha}.
- Programaciones: ahora, fecha/hora exacta, semanal jueves-jueves, semanal lunes-lunes, quincenal y mensual.
- Activar/pausar, editar y eliminar programaciones desde la pantalla.
- Bitácora de envíos con fecha, destinatarios, reportes, estado y error.
- Generación de reportes tabulares en Excel (.xlsx) con encabezado Constructora AG.
- Generación de reporte gráfico del Dashboard en PDF.
- Dashboard: nueva gráfica de indicadores en tiempo real con usuarios activos, check-in y porcentaje de asistencia por día, semana y mes.

Nota técnica:
La pantalla guarda configuración local en AppData/ConstructoraAG/ReportesCorreo/config_reportes_correo.json.
Para ejecución automática real en segundo plano, conectar la lista de programaciones a un servicio Windows o tarea programada que invoque EmailReportService según la frecuencia configurada.
