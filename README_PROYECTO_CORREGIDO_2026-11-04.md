# Proyecto corregido consolidado - 2026-11-04

Rama basada en `feature/horarios-paginacion50-exportacion`.

Incluye:
- paginación de 50 registros en Horarios empleados;
- exportación Excel/CSV de todos los resultados consultados;
- eliminación del límite TOP (500);
- corrección de compilación del método CSV;
- selección inteligente de entrada según horario asignado;
- salida tomada sólo de marcaciones posteriores a la entrada seleccionada;
- fallback a la lógica histórica cuando el empleado no tiene horario.

No se modifican registros originales de checada ni tablas de asistencia.

Nota: la versión local mostrada en capturas contiene archivos/cambios que no están respaldados en GitHub. Esta rama consolida únicamente lo que sí existe en el repositorio.
