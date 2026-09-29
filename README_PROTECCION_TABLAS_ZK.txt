CORRECCIÓN: MÚLTIPLES BASES Y PROTECCIÓN EXCLUSIVA DE TABLAS ZK

TABLAS ORIGINALES PROTEGIDAS
Se protege la lista completa proporcionada por el usuario, desde dbo.ac_group hasta dbo.zkproto_sync_queue. La lista exacta está centralizada en Services/ZkTableProtection.cs e incluye tablas ac_*, att_*, hr_*, sys_*, colas ZK y las tablas ZK auxiliares indicadas.

Estas tablas sólo se consultan. El sistema bloquea INSERT, UPDATE, DELETE, ALTER, DROP, CREATE, TRUNCATE, REPLACE y MERGE cuando la sentencia hace referencia a cualquiera de ellas dentro de una base registrada como checador.

LAS DEMÁS TABLAS SÍ SE PUEDEN MODIFICAR
Las tablas propias de BDSistemas, como Empleados, configuraciones, incidencias administrativas, nómina, catálogos, bitácora, respaldos y reportes, conservan sus operaciones normales. Si una tabla administrativa tiene el mismo nombre que una tabla ZK, la protección sólo se activa cuando servidor, puerto y base coinciden con una conexión registrada como checador.

FUNCIONAMIENTO
1. El usuario agrega desde Conexiones todas las bases de datos y ubicaciones necesarias.
2. No se incluye ninguna base de checador preconfigurada.
3. Asistencia consulta por separado las conexiones activas y consolida los resultados dentro de la aplicación.
4. La relación entre usuarios ZK y empleados se realiza por PIN dentro de la aplicación.
5. Usuarios de checadores muestra ID, PIN, nombre, área, puesto, dispositivo, ubicación, conexión y base de origen.
6. No se agregan columnas, vistas ni relaciones a las tablas originales de ZK.
7. La protección se aplica según la base de destino, evitando bloquear por error tablas propias de BDSistemas.

VERIFICACIÓN
- No se estableció conexión con bases reales durante esta corrección.
- No se ejecutó ninguna sentencia SQL contra bases reales.
- No se modificó estructura ni información de tablas ZK.
- Las demás tablas del sistema no fueron convertidas a sólo lectura.
