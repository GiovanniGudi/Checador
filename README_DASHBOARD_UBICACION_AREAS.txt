Cambios aplicados al Dashboard:
- Nueva gráfica: Usuarios activos vs usuarios que checaron hoy por ubicación.
  * Activos: empleados activos de dbo.Empleados agrupados por ubicación.
  * Checaron hoy: empleados con al menos una checada del día natural actual, agrupados por ubicación.
- Nueva gráfica: Asistencias por área del año actual.
  * Cuenta asistencias únicas por empleado y día durante el año actual.
  * Agrupa los resultados por área.
- Las consultas son solo lectura sobre tablas ZK:
  dbo.att_punches y dbo.hr_employee.
- No se modifican tablas protegidas ni se agregan columnas.
