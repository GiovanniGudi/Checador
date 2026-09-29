V25 - Corrección filtro categorías en reportes y jefe directo

Cambios:
1) En Reportes por correo, al usar Enviar ahora se sincronizan los checks actuales de reportes y categorías con el/los destinatarios activos.
2) El Excel de asistencias enviado por correo filtra realmente por categoría seleccionada.
3) El resumen del correo también cuenta asistencias e incidencias respetando categorías.
4) La hoja Asistencias del Excel agrega la columna Jefe directo.
5) La hoja Empleados Activos agrega la columna Jefe directo.
6) En la tabla visual de Consulta de Asistencia se agregó Jefe directo y se ocultaron Descuento y Pago estimado.

Notas:
- No se modifican tablas biométricas ni procesos de sincronización.
- Solo se consulta dbo.att_punches, dbo.hr_employee y tablas relacionadas del checador en modo lectura.
- La columna de jefe directo del sistema sigue usando dbo.Empleados.jefe_directo_id.
