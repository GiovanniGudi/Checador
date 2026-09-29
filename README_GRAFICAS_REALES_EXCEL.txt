Corrección aplicada:
- Ahora el Excel no solo genera tablas: también inserta imágenes PNG de gráficas dentro de las hojas.
- Las gráficas se generan desde C# con WPF RenderTargetBitmap y se insertan con ClosedXML AddPicture.
- Reportes por correo:
  * Dashboard KPI contiene gráficas reales por ubicación, área y jefe.
- Exportación manual de Asistencia:
  * Dashboard KPI contiene gráficas reales por ubicación y área.
- Se mantienen las tablas de respaldo para validar los datos que alimentan cada gráfica.
- No se modifican tablas protegidas ZK; solo lectura.
