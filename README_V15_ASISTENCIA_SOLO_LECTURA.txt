CORRECCIÓN V15 - CONSULTA DE ASISTENCIAS SÓLO LECTURA

Archivo SQL a ejecutar:
SQL\Patch_V15_Asistencia_Consulta_SoloLectura.sql

Este patch modifica únicamente la vista permitida:
dbo.vw_asistencia_integral

No modifica tablas protegidas de ZK/Time.Net.
Las tablas dbo.att_punches, dbo.att_terminal, dbo.hr_employee, etc. sólo se leen con SELECT.

Cambios en el módulo:
- Entrada = primer registro del día.
- Salida = último registro del día.
- Jornada requerida fija = 10 horas.
- Muestra registros incompletos.
- Muestra registros con una sola marcación.
- Muestra empleados sin registros.
- Muestra empleados activos e inactivos.
- Muestra ID y nombre del checador.
- Muestra ID y nombre del sistema interno.
- Muestra terminal desde dbo.att_terminal.terminal_name.
- Evita el error Invalid column name 'Terminal'.
- Agrega filtros por texto, estado, ubicación, puesto, inconsistencias, jornadas incompletas, sin registros y con registros.

IMPORTANTE:
Ejecutar únicamente Patch_V15_Asistencia_Consulta_SoloLectura.sql para esta mejora.
