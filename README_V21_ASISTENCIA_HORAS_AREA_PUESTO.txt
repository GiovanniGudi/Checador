V21 - Consulta de asistencia solo lectura

Cambios:
- Match entre checador y tabla Empleados únicamente por PIN.
- Muestra Fecha, Hora Entrada, Hora Salida y Tiempo/Horas Trabajadas.
- Jornada requerida: 10:00:00.
- Menos de 10 horas = Incompleto.
- Exactamente 10 horas = Completo.
- Más de 10 horas = Horas extra.
- Muestra Horas Extra y Horas Faltantes.
- Agrega Área y Puesto desde dbo.Empleados.

Importante:
- No modifica tablas protegidas.
- No ejecuta INSERT, UPDATE, DELETE, ALTER, DROP ni CREATE sobre tablas del checador.
- La consulta de asistencia se realiza en código con SELECT.
