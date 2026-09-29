Módulo de Asistencia - Respaldo de registros

Cambios:
- Se agregó botón "Respaldar registros" en el módulo de Asistencia.
- Se agregó leyenda "Último respaldo de asistencia".
- El respaldo toma el rango seleccionado en Fecha inicial y Fecha final.
- Los datos se copian desde las tablas de historial del checador a tablas propias del sistema:
  dbo.AppAsistenciaRespaldos
  dbo.AppAsistenciaRespaldosDetalle
- No modifica tablas del checador/ZKTeco.
- Evita duplicar registros usando IdPunch como único.
- También genera un Excel del respaldo en AppData/ConstructoraAG/RespaldosAsistencia.
