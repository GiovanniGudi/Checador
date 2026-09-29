Correcciones aplicadas:
- En Empleados ya NO se cargan todos los puestos al iniciar.
- El combo Puesto inicia vacío y solo se llena cuando se selecciona un Área.
- Si el área no tiene puestos configurados, el combo de Puesto queda vacío.
- Se modificó el módulo Catálogos para administrar:
  1) Catálogos generales: Área, Puesto y Ubicación.
  2) Puestos por área: relación Área -> Puestos permitidos.
- Ubicación sigue existiendo y se administra independiente; no afecta el filtrado de puestos.
- Tabla propia usada para la relación:
  dbo.AppAreaPuestos
