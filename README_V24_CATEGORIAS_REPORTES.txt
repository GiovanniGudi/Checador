V24 - Categorías de empleados y reportes por correo

Cambios incluidos:
1. Jefe directo muestra ID - Nombre Completo.
2. Nueva columna dbo.Empleados.categoria.
3. Nuevo módulo: Categorías empleados.
4. Eliminación de empleados con confirmación y bitácora, solo en dbo.Empleados.
5. Reportes por correo sin límite de 10 destinatarios.
6. Importación de destinatarios desde TXT/CSV.
7. Filtro por categorías en reportes por correo.
8. Excel de asistencia con hoja Asistencias y hoja Empleados Activos.

Restricción respetada: no se modifican tablas biométricas ni procesos de sincronización.
Ejecuta SQL/Patch_V24_Categorias_Empleados_Reportes.sql antes de usar el módulo si deseas preparar la BD manualmente. La app también valida la estructura al abrir módulos.
