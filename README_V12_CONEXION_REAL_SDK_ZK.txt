PSSF V12 - CONEXION REAL MEDIANTE SDK ZKTECO

Correccion principal:
- Se elimino la prueba TcpClient al puerto 4370 como requisito previo para sincronizar.
- El estado En linea/Sin conexion ahora depende de la conexion real del SDK ZKTeco (Connect_Net).
- Esto evita falsos "Sin conexion" en equipos que funcionan con ZKTime.Net pero no aceptan una prueba TCP generica.
- Se conserva el timeout de 30 segundos para que un checador bloqueado no detenga a los demas.
- No se modifica la estructura SQL ni la logica de reportes.
- Se conserva el control de duplicados de la importacion existente.

Prueba recomendada:
1. Cerrar PSSF anterior.
2. Abrir esta version.
3. Esperar un ciclo completo.
4. Revisar el texto Detalle de cada checador. Si Connect_Net falla, ahora se mostrara el error real del SDK y no un falso resultado de TcpClient.
