V22 - Consulta de asistencia

Cambios:
- El estado se toma exclusivamente de dbo.Empleados.estado.
- Se agrega filtro Estado empleado: Todos, Activos, Inactivos.
- El match entre checador y Empleados se hace por PIN:
  dbo.hr_employee.emp_pin = dbo.Empleados.id_checador_principal
- La base del reporte son los registros/empleados del checador.
- Si existe en checador y no existe en dbo.Empleados, se muestra de todos modos.
- No modifica tablas protegidas. Sólo ejecuta SELECT.

Columnas principales:
- Fecha
- PIN Checador (tabla checador)
- Nombre Checador (tabla checador)
- PIN Empleado (tabla Empleados)
- Nombre Sistema (tabla Empleados)
- Estado Empleado (tabla Empleados)
- Área
- Puesto
- Hora Entrada
- Hora Salida
- Horas trabajadas
- Horas extra
- Horas faltantes
- Estatus jornada
