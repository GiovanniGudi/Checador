PSSF V19 - USUARIOS DE CHECADORES: NÚMERO DE EMPLEADO VISIBLE

Cambio solicitado:
- Se conserva la columna PIN proveniente de la tabla original hr_employee de cada checador.
- Se agrega una columna independiente: "Número empleado (tabla Empleados)".
- Ese valor se toma de dbo.Empleados.pin_empleado después de realizar el match.
- Así se pueden comparar visualmente PIN del checador vs. número de empleado de la tabla Empleados.
- Se conservan ID empleado, nombre, ubicación, categoría, puesto, área, terminal y última checada.
- El buscador también contempla el nuevo campo.
- La exportación a Excel incluye el nuevo dato automáticamente.
- No se modifican tablas de los checadores; solo se realizan consultas de lectura.
