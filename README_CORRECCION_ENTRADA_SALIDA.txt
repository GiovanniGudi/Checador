Corrección de Entrada y Salida en Consulta de Asistencia

Ejecutar en SQL Server:
SQL\Patch_V13_Asistencia_EntradaSalida_PorPIN.sql

Qué corrige:
- La consulta ya no depende solamente de id_checador_principal/id_checador_secundario.
- Ahora también cruza las checadas por PIN: hr_employee.emp_pin = Empleados.pin_empleado.
- Esto permite que aparezcan Hora entrada y Hora salida aunque el ID interno del checador no esté guardado correctamente en Empleados.
- Mantiene Terminal usando att_punches.terminal_id = att_terminal.id.

Tabla/vista que usa la pantalla:
- dbo.vw_asistencia_integral
