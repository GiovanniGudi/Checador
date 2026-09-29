CAMBIO: Gráficas de empleados activos vs asistencia por ubicación

Se modificó Services/EmailReportService.cs para que los reportes Excel de:
- Asistencias
- Empleados activos
- Dashboard

incluyan una hoja llamada "Activos vs Asistencia" con el resumen por ubicación y, además, una hoja individual por cada ubicación encontrada, por ejemplo:
- Grafica Ubicacion 1
- Grafica Ubicacion 2
- Grafica SIN UBICACIÓN

Cada hoja compara:
- Empleados activos
- Checaron asistencia hoy
- Checaron asistencia en el mes actual
- % asistencia hoy
- % asistencia mes

La gráfica se genera dentro del Excel como barras visuales en celdas, compatible con ClosedXML sin agregar paquetes nuevos.

Notas técnicas:
- Los empleados activos se agrupan por dbo.Empleados.ubicacion.
- La asistencia se toma de dbo.att_punches y se enlaza con dbo.Empleados usando id_checador_principal.
- El día actual se calcula con DateTime.Today.
- El mes actual se calcula desde el primer día del mes hasta el primer día del siguiente mes.
