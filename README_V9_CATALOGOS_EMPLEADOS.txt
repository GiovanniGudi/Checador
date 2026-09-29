Constructora AG WPF V9 - Catálogos aplicados en Empleados

Correcciones:
1. El módulo Catálogos ahora sí se usa en Empleados:
   - Área es ComboBox desde catálogo.
   - Puesto es ComboBox desde catálogo.
   - Ubicación es ComboBox desde catálogo.

2. En Empleados ahora se muestra claramente:
   - ID Checador Principal
   - ID Checador Secundario
   - Registrado en:
     * Solo Checador Principal
     * Solo Checador Secundario
     * Ambos Checadores
     * Sin registro en Checadores

3. El DataGrid de empleados tiene columnas explícitas para que siempre aparezca "Registrado en".

Pasos:
1. Ejecuta SQL/Patch_V9_Catalogos_Empleados.sql en Checador.
2. Abre ConstructoraAG_WPF_Listo.sln.
3. dotnet restore
4. dotnet build
5. Ejecuta.
