V20 - Asistencia con match por PIN

Reglas aplicadas:
- El reporte toma como base el checador: dbo.hr_employee y dbo.att_punches.
- El match con dbo.Empleados se hace únicamente por PIN:
  dbo.hr_employee.emp_pin = dbo.Empleados.id_checador_principal
- No se usa el ID interno hr_employee.id para vincular contra Empleados.
- Si una persona existe en checador y también en Empleados con el mismo PIN, aparece en la misma fila.
- Si existe en checador pero no existe en Empleados, aparece con datos de Empleados vacíos.
- Si existe en Empleados pero no existe en checador, no aparece.
- No modifica tablas protegidas; sólo ejecuta SELECT.
