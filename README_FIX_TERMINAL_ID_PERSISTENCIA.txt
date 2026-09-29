CORRECCION - CHECADORES ZK / TERMINAL ID QUE DESAPARECIA
Fecha: 25/09/2026

Problema corregido:
- Al agregar un nuevo checador (por ejemplo OdataCheck03, Terminal ID 5), aparecia inicialmente en la tabla.
- Segundos despues o al recargar la vista podia desaparecer.

Causa:
- La sincronizacion automatica cargaba una copia anterior de checadores_zk.json.
- Al terminar la sincronizacion guardaba nuevamente TODA esa lista anterior.
- Si durante ese intervalo se habia agregado un checador nuevo, la lista antigua sobrescribia el archivo y eliminaba el nuevo registro.

Correccion aplicada:
1. ChecadoresZkService ahora sincroniza el acceso al archivo de configuracion.
2. Guardar un checador actualiza solamente ese registro (por Guid interno), sin sobrescribir otros checadores.
3. Eliminar elimina solamente el registro seleccionado.
4. La sincronizacion automatica guarda UNICAMENTE campos de estado:
   - EnLinea
   - UltimaRevision
   - UltimaSincronizacion
   - UltimosRegistrosInsertados
   - UltimoError
   No puede sobrescribir Nombre, IP, ConexionId, TerminalId ni eliminar checadores nuevos.
5. La revision de estados usa la misma estrategia de combinacion.
6. La escritura de checadores_zk.json se hace mediante archivo temporal para reducir riesgo de JSON incompleto.
7. La solucion de Visual Studio fue alineada a Debug|x86 y Release|x86.
8. Se elimino Prefer32Bit porque no aplica a .NET 8; PlatformTarget sigue siendo x86.

Configuracion esperada:
- OdataCheck01 -> Terminal ID 3
- OdataCheck02 -> Terminal ID 4
- OdataCheck03 -> Terminal ID 5
- Check_IT     -> Terminal ID 1005

Archivos principales modificados:
- Services/ChecadoresZkService.cs
- Services/ZkAutoSyncService.cs
- Views/ChecadoresZkView.xaml.cs
- ConstructoraAG_WPF_Listo.sln
- ConstructoraAG_WPF_Listo.csproj

Nota:
No se pudo ejecutar dotnet build en el entorno de correccion porque no dispone del SDK .NET. El proyecto esta preparado para compilar en Windows con .NET 8 en x86.
