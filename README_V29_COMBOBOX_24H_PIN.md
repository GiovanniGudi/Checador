# V29 - ComboBox legible y cálculo por PIN/día natural

Cambios realizados:

- Los ComboBox mantienen fondo blanco con texto negro, incluyendo el valor seleccionado.
- Se conservan TextBox, DatePicker y calendarios con fondo blanco y texto negro.
- En asistencia, el cálculo de Entrada/Salida se consolida por PIN del checador, no por registros duplicados de `hr_employee.id`.
- Entrada = primer registro del día natural 00:00:00 a 23:59:59.
- Salida = último registro del mismo día natural, sólo si existe más de una marcación ese día.
- Una marcación a la 1:00 AM pertenece al nuevo día; no se toma como salida del día anterior.
- No se modifican tablas restringidas. Sólo consultas SELECT.
