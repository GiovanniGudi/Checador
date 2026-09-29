V16 - Consulta de Asistencias sólo lectura

Corrección aplicada:
- El módulo ya NO consulta columnas con alias dentro de dbo.vw_asistencia_integral.
- Se corrigió el error: Invalid column name 'Estado Sistema', 'ID Checador', 'Nombre Checador', etc.
- La consulta se calcula directamente con SELECT desde:
  dbo.Empleados
  dbo.hr_employee
  dbo.att_punches
  dbo.att_terminal

Importante:
- NO modifica tablas protegidas.
- NO ejecuta UPDATE, DELETE, INSERT, TRUNCATE, ALTER TABLE, DROP TABLE ni CREATE TABLE.
- NO requiere modificar dbo.vw_asistencia_integral para que cargue el módulo.

Reglas aplicadas:
- Entrada = primer registro del día natural.
- Salida = último registro del día natural, si hay más de una marcación.
- Si sólo hay una marcación, se muestra como Entrada y el estatus queda Sin Salida.
- Jornada requerida = 10:00:00.
- Se muestran empleados con registro, sin registro, activos e inactivos.
- Se muestra terminal usando dbo.att_terminal.terminal_name por medio de dbo.att_punches.terminal_id.

Relación usada:
- dbo.att_punches.employee_id se relaciona con dbo.hr_employee.id.
- dbo.Empleados.id_checador_principal se intenta relacionar con dbo.hr_employee.id o con dbo.hr_employee.emp_pin.
  Esto evita que se crucen registros equivocados cuando Time.Net muestra el PIN como ID de usuario.

Archivo principal corregido:
- Services/AsistenciaService.cs
