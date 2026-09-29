CORRECCIÓN DE CONSULTA DE ASISTENCIAS - TERMINAL

Archivo SQL a ejecutar:
SQL\Patch_V12_Asistencia_Terminal_Nombre.sql

Qué corrige:
- La consulta de asistencia usa la vista dbo.vw_asistencia_integral.
- att_punches guarda terminal_id, no guarda el nombre.
- El nombre se obtiene desde att_terminal.terminal_name.
- Relación usada:
  att_punches.terminal_id = att_terminal.id

Después de ejecutar el patch, la vista dbo.vw_asistencia_integral tendrá la columna [Terminal]
con valores como OdataCheck01 u Odata_Check02.
