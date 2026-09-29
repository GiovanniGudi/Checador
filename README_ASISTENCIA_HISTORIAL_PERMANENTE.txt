Cambio aplicado:
- El respaldo de asistencia ahora se guarda en tablas nuevas y permanentes de SQL:
  dbo.AppAsistenciaHistorialRespaldos
  dbo.AppAsistenciaHistorial

Objetivo:
- Si posteriormente se borra o limpia la asistencia original del checador, el historial respaldado permanece en estas tablas.
- El módulo Historial de Respaldos consulta estas tablas propias, no depende de dbo.att_punches después del respaldo.

Importante:
- No se modifican tablas ZKTeco/checador.
- Solo se leen dbo.att_punches, dbo.hr_employee y dbo.att_terminal al momento de respaldar.
- Los registros respaldados no se duplican porque IdPunch queda como único.
