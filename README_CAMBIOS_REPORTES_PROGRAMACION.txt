Cambios aplicados:
- Se elimina la lógica de "período a generar" como selector independiente.
- Enviar ahora mismo genera el reporte del día actual y lo envía de inmediato.
- Cada destinatario puede tener su propia frecuencia y sus propios reportes:
  Diario, Lunes a viernes, Jueves a jueves, Lunes a lunes, Quincenal, Mensual, Anual o Fecha exacta.
- Jueves a jueves calcula el rango completo de semana iniciando en jueves.
- Mensual calcula del primer al último día del mes.
- Anual calcula del 1 de enero al 31 de diciembre.
- Los checks de reportes solo indican qué tablas se adjuntan.
- Se corrigió la generación del PDF del Dashboard para incluir gráficas simples de barras y evitar caracteres corruptos.
- No se modifican tablas del checador/ZKTeco; se consultan solo con SELECT.
