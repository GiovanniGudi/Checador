V27
- Mantiene tema corporativo oscuro.
- TextBox, ComboBox, ComboBoxItem, DatePicker y calendario quedan con fondo blanco y texto negro.
- Se quitó texto blanco en campos de captura con fondo blanco.
- Asistencia mantiene lógica de día natural: cada registro se agrupa por CAST(punch_time AS date).
  Si un empleado checa hoy a las 08:00 y no checa salida, una checada de mañana, aunque sea 01:00 AM,
  NO se toma como salida del día anterior.
- No se modifican tablas restringidas; sólo consultas SELECT.
