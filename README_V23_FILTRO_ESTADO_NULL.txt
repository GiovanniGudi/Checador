V23 - Filtro Estado Empleado con opción NULL

Cambios realizados:
- Se agregó la opción NULL al filtro Estado empleado en Consulta de Asistencia.
- El filtro Todos muestra Activo, Inactivo y NULL/vacío.
- El filtro NULL muestra registros donde EstadoEmpleado es NULL o cadena vacía.

Restricciones respetadas:
- No se modifican tablas protegidas.
- No se ejecutan INSERT, UPDATE, DELETE, ALTER, DROP ni TRUNCATE sobre tablas del checador.
- La consulta sigue siendo de solo lectura.
