CORRECCIÓN ASISTENCIA MULTI-BD V3

Problema corregido:
La versión V2 podía dejar la consulta de Asistencia sin registros al cambiar la selección de conexiones usada por QueryAllReadOnlyAsync.

Cambio aplicado:
- QueryAllReadOnlyAsync vuelve a recorrer TODAS las conexiones ACTIVAS con LoadActive().
- Se conserva la consolidación por PIN + fecha.
- Se conserva el recálculo global de entrada/salida usando marcaciones de todas las BD activas.
- Se conservan las terminales de todas las BD en una sola jornada.

No requiere cambios en las tablas SQL.
