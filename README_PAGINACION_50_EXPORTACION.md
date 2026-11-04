# Horarios: paginación 50 + exportación completa

Cambios aplicados sobre `checkpoint/horarios-centralizados-2026-11-04`.

- 50 registros por página.
- Primera / Anterior / Siguiente / Última.
- Indicador de página y total consultado.
- Una búsqueda vuelve a la página 1.
- Excel y CSV exportan todos los registros devueltos por el filtro, no sólo los 50 visibles.
- Se eliminó `TOP (500)` de la consulta para no truncar exportaciones.
- No se cambiaron fuentes ni estilos globales.

Nota: la pantalla local más reciente mostrada en captura ("Administrador de horarios") no está todavía almacenada en este checkpoint; esta rama contiene el proyecto completo disponible en GitHub con la mejora aplicada a su módulo de horarios.
