Corrección aplicada en Consulta de Asistencia:
- Horas trabajadas ya se muestran en horas y minutos, no como horas completas.
- El cálculo de horas trabajadas se mantiene dinámico:
  primer registro del día - último registro del día.
- Horas extra también se muestran en horas y minutos.
- Horas faltantes y horas requeridas también se muestran en horas y minutos.
- Ejemplo:
  Entrada 08:03 y salida 17:41 = 9 h 38 min.
  Jornada requerida 10 h 00 min.
  Horas faltantes 0 h 22 min.
- Se respetan las horas requeridas configuradas por día en Jornada Laboral.
- No se modifican tablas ZKTeco/checador. Solo se leen registros.
