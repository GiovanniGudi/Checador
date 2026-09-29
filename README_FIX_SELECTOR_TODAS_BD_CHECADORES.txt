CORRECCIÓN - SELECTOR DE BASES EN CHECADORES ZK

Cambios realizados:
1. El ComboBox "Base de datos destino" muestra TODAS las conexiones configuradas en Conexiones.
2. La marca "BD administrativa" no filtra ni reemplaza las bases disponibles para checadores.
3. El selector se recarga desde conexiones.json cada vez que se abre el desplegable.
4. La descarga usa exclusivamente el ConexionId asignado al checador.
5. Se eliminó el filtro por Activa al resolver la BD destino del checador, para que no desaparezca una conexión ya configurada por una marca de estado.
6. No se usa la BD administrativa como fallback. Si el ConexionId no existe, se cancela la descarga para evitar insertar en una BD incorrecta.

IMPORTANTE:
Después de abrir el proyecto, vaya a Checadores ZK, seleccione el checador, elija su BD destino y pulse Guardar.
