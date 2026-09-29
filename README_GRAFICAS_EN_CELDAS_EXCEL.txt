Corrección aplicada:
- Se eliminaron las gráficas como imágenes PNG y todo uso de MemoryStream / WPF RenderTargetBitmap.
- Las gráficas ahora se generan dentro del Excel usando celdas, valores y barras visuales con caracteres, sin imágenes.
- Cada gráfica incluye una descripción clara sobre lo que representa.
- Se conservan las hojas KPI:
  * Dashboard KPI
  * KPI por ubicación
  * KPI por área
  * KPI por jefe
  * KPI periodos
  * Top faltas
  * Top retardos
  * Hojas por ubicación
- No se modifican tablas protegidas ZK; solo se generan reportes de lectura.
