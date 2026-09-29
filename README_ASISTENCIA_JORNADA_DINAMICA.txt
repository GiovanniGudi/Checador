Corrección aplicada en Asistencia:
- La consulta ya no deja fija la jornada en 10 horas.
- Toma las reglas del módulo Jornada laboral:
  * días laborables,
  * horas por día,
  * hora de entrada,
  * hora de salida,
  * minutos de tolerancia para retardo,
  * minutos permitidos para entrada anticipada.
- Se recalculan por día:
  * Horas requeridas,
  * Horas extra,
  * Horas faltantes,
  * Estatus de jornada,
  * Retardo en minutos,
  * Entrada anticipada en minutos,
  * Descuento aplicado,
  * Pago estimado.
- Sábado puede ser 5 horas y domingo no laboral según configuración.
- Si cambia la hora de entrada/salida en Jornada laboral, Asistencia la respeta al volver a buscar.
- No modifica tablas ZKTeco/checador; solo lee asistencias y empleados.
