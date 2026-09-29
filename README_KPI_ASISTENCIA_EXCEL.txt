Cambios agregados:
- El Excel de reportes de asistencias enviados por correo ahora incluye:
  * Hoja Dashboard KPI como segunda hoja.
  * Usuarios activos vs usuarios con entrada/salida.
  * KPI por ubicación.
  * KPI por área.
  * KPI por jefe directo.
  * KPI día, semana, mes y periodo del reporte.
  * Top faltas.
  * Top retardos.
  * Hojas separadas por ubicación.
- El módulo de consulta de asistencias, al exportar Excel, también agrega Dashboard KPI y hojas por ubicación con base en los datos visibles de la consulta.
- No se agregan columnas ni se modifican tablas protegidas ZK. Las consultas son de solo lectura sobre att_punches/hr_employee y datos propios de Empleados/Incidencias.
- No se incluyen temas de nómina, solo asistencia y fuerza de trabajo.
Nota técnica:
- ClosedXML no genera objetos de gráfica nativos en todas las versiones; por compatibilidad se agregan tablas KPI y barras visuales dentro de celdas para los comparativos.
