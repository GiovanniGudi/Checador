CORRECCIÓN – SELECCIÓN DE BASE DE DATOS ADMINISTRATIVA

Problema corregido:
Al marcar una conexión como "BD administrativa", el sistema podía seguir utilizando
o mostrando la otra conexión como administrativa.

Causas corregidas:
1. La selección administrativa se conservaba en caché y no se actualizaba al cambiarla.
2. La lógica de la vista dependía del Id para desmarcar las demás conexiones; una
   configuración antigua con Id duplicados podía dejar marcada la conexión incorrecta.
3. Se agregó normalización para reparar Id duplicados automáticamente.

Comportamiento nuevo:
- Sólo una conexión puede quedar marcada como administrativa.
- La conexión seleccionada por el usuario es la que queda guardada.
- El cambio se aplica de inmediato sin tener que reiniciar la aplicación.
- Las demás conexiones activas siguen disponibles como bases de checadores.

Archivos modificados:
- Views/ConexionesView.xaml.cs
- Services/ConfigService.cs
- Services/DatabaseService.cs
