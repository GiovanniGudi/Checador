FIX - Reporteo por correo no debe cerrar el sistema

Cambios aplicados:
1. EmailReportService.Cargar() ahora valida config_reportes_correo.json.
   - Si el archivo no existe, está vacío o está corrupto, crea configuración nueva.
   - Si está corrupto, intenta respaldarlo como .corrupto_yyyyMMdd_HHmmss.bak.
2. ReportesCorreoView ahora captura errores durante Loaded/CargarAsync para que la app no se cierre al entrar al módulo.
3. MainWindow.Nav_Click ahora protege la navegación entre módulos con try/catch. Si un módulo falla, muestra aviso y regresa al Dashboard sin cerrar el sistema.

No se modificó base de datos, lógica de otros módulos ni interfaz general.
