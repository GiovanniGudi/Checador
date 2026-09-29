CORRECCIÓN ASISTENCIA - 1 O MÁS BASES DE CHECADORES

Problema corregido:
- La vista de Asistencia podía terminar mostrando únicamente empleados/marcaciones de una base,
  aunque hubiera dos o más conexiones de checadores configuradas.

Cambios:
1. QueryAllReadOnlyAsync recorre explícitamente todas las conexiones de checadores activas.
2. Asistencia carga además el catálogo mínimo dbo.hr_employee de todas las bases activas.
3. Si una consulta completa de una BD no aporta filas, sus empleados se incorporan desde hr_employee.
4. Las marcaciones individuales se vuelven a consultar desde todas las BD y se consolidan por PIN + fecha.
5. Entrada = primera marcación global del día entre todas las BD.
6. Salida = última marcación global del día entre todas las BD.
7. Terminal = todas las terminales realmente utilizadas, aunque pertenezcan a BD distintas.
8. Funciona con 1, 2, 3 o más bases; no existe una BD principal para la asistencia.

No requiere cambios SQL ni modificaciones de tablas.
