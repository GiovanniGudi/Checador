using System.Windows;
using System.Windows.Controls;
using ConstructoraAG_WPF_Listo.Views;
using ConstructoraAG_WPF_Listo.Services;

namespace ConstructoraAG_WPF_Listo;

public partial class MainWindow : Window
{
    private readonly string _usuario;
    private readonly EmailSchedulerService _emailScheduler = new();
    private readonly ReporteActividadSchedulerService _reporteActividadScheduler = new();
    private readonly HistorialChecadasSchedulerService _historialChecadasScheduler = new();
    private readonly ZkAutoSyncService _zkAutoSync = ZkAutoSyncService.Instance;

    public MainWindow(string usuario)
    {
        InitializeComponent();

        _usuario = usuario;
        TxtUsuario.Text = "Usuario: " + usuario;

        // IMPORTANTE: no bloquear el hilo UI con GetAwaiter().GetResult().
        // Las migraciones administrativas se ejecutan al cargar la ventana de forma asíncrona.
        Loaded += MainWindow_Loaded;
        Closed += (_, _) =>
        {
            _zkAutoSync.Stop();
            _historialChecadasScheduler.Dispose();
        };
    }


    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Migraciones mínimas y exclusivamente administrativas.
            // Nunca se ejecutan escrituras en las bases de checadores.
            await new EmpleadoService().AsegurarEstructuraAsync();
            await new ConfiguracionWhatsAppService().AsegurarEstructuraAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "La sesión inició correctamente, pero no se pudo preparar la estructura administrativa.\n\n" +
                ex.Message,
                "Inicialización administrativa",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        try
        {
            MainContent.Content = new DashboardView();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "La sesión inició, pero no se pudo cargar el Dashboard.\n\n" + ex.Message,
                "Dashboard",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        _emailScheduler.Start();
        _reporteActividadScheduler.Start();
        _historialChecadasScheduler.Start();

        // Iniciar descarga automática de los checadores guardados al abrir la aplicación.
        // Antes sólo se iniciaba al pulsar Guardar en la vista de checadores.
        try { _zkAutoSync.Start(); }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo iniciar la sincronización ZKTeco: " + ex.Message,
                "Sincronización ZKTeco", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Nav_Click(object sender, RoutedEventArgs e)
    {
        var tag = (sender as System.Windows.Controls.Button)?.Tag?.ToString();

        try
        {
            MainContent.Content = tag switch
            {
                "Dashboard" => new DashboardView(),
                "Empleados" => new EmpleadosView(),
                "Asistencia" => new AsistenciaView(),
                "AsistenciaSemanal" => new AsistenciaSemanalView(),
                "PersonalSinAlta" => new PersonalSinAltaView(),
                "RangosEmpleado" => new RangosEmpleadoView(),
                "HistorialRespaldoAsistencia" => new HistorialRespaldoAsistenciaView(),
                "HistorialChecadas" => new HistorialChecadasView(),
                "Incidencias" => new IncidenciasView(_usuario),
                "Conexiones" => new ConexionesView(),
                "UsuariosChecadores" => new UsuariosChecadoresView(),
                "ChecadoresZk" => new ChecadoresZkView(),
                "Catalogos" => new CatalogosView(_usuario),
                "CategoriasEmpleado" => new CategoriasEmpleadoView(),
                "AreaPuestos" => new AreaPuestosView(),
                "Nomina" => new NominaView(_usuario),
                "Bitacora" => new BitacoraView(),
                "ReportesCorreo" => new ReportesCorreoView(),
                "ConfiguracionCorreoReporteActividad" => new ConfiguracionCorreoReporteActividadView(),
                "ReporteActividadEmpleados" => new ReporteActividadEmpleadosView(),
                "JornadaLaboral" => new JornadaLaboralView(),
                "HorariosEmpleados" => new HorariosEmpleadosView(_usuario),
                "ConfiguracionWhatsApp" => new ConfiguracionWhatsAppView(),
                _ => new DashboardView()
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show("No se pudo abrir el módulo seleccionado. El sistema seguirá abierto.\n\nDetalle técnico: " + ex.Message,
                "Error al abrir módulo", MessageBoxButton.OK, MessageBoxImage.Warning);
            MainContent.Content = new DashboardView();
        }
    }
}
