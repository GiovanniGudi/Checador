CORRECCIÓN: DESCARGAS A LA BD ASIGNADA POR CHECADOR

Problema corregido:
- La marca "BD administrativa" estaba excluyendo esa conexión de la lista de bases de checadores.
- Esto podía provocar que no estuvieran disponibles todas las BD activas para asignarlas a cada checador y que las consultas consolidadas no contemplaran la administrativa cuando también contiene datos ZK.

Cambios:
1. ConfigService.LoadChecadores() ahora devuelve TODAS las conexiones activas, incluida la administrativa.
2. ZkAttendanceImportService.ImportarAsync() usa estrictamente el ConexionId del checador.
3. Si la BD asignada no existe o está inactiva, la descarga se cancela; NO se redirige a la administrativa.
4. El selector de BD en Checadores ZK muestra todas las conexiones activas.

Resultado esperado:
- Cada checador descarga únicamente a la BD que tenga seleccionada/asignada en Checadores ZK.
- ChecadorV2 puede ser administrativa y, al mismo tiempo, ser destino de uno o varios checadores.
- BDCHECKSM puede ser destino de otros checadores sin necesidad de marcarla administrativa.
