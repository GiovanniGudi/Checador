CORRECCIÓN – BD ADMINISTRATIVA / SELECCIÓN EXACTA

Problema:
Al seleccionar una conexión como “BD administrativa”, podía terminar utilizándose o
mostrándose la conexión contraria.

Corrección aplicada:
- La conexión administrativa se asigna por el Id exacto de la fila seleccionada.
- Primero se reemplaza/crea la conexión y después se fuerza que sólo ese Id quede marcado.
- ConfigService.SaveAll acepta el Id administrativo explícito y vuelve a aplicarlo al persistir.
- La vista recarga desde disco y vuelve a seleccionar el mismo Id guardado.
- DatabaseService.GetConnection() usa LoadAdministrativa() en lugar de la primera conexión activa.
- Se aplicó la misma corrección a la copia anidada del proyecto para evitar ejecutar una versión distinta.

No se modificaron otros módulos ni reglas del sistema.
