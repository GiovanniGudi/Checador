Cambio aplicado:
- Se agregó relación de Puestos por Área mediante tabla propia:
  dbo.AppAreaPuestos
- Se agregó módulo "Puestos por área" para configurar qué puestos pertenecen a cada área.
- En el módulo Empleados, al seleccionar un Área, el combo de Puesto se filtra automáticamente.
- Si un área no tiene puestos configurados, se muestran todos los puestos para no bloquear el alta.
- Al guardar un empleado, si se captura un área y puesto, la relación se aprende/guarda automáticamente.
- No se modifican tablas ZKTeco/checador.
