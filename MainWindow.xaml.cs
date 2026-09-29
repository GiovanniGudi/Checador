using System.Windows;
using System.Windows.Controls;
using ConstructoraAG_WPF_Listo.Views;
using ConstructoraAG_WPF_Listo.Services;

namespace ConstructoraAG_WPF_Listo;

public partial class MainWindow : Window
{
    private readonly string _usuario;
    private readonly EmailSchedulerService _emailScheduler = new();
    private bool _menuVisible = true;

    public MainWindow(string usuario)
    {
        InitializeComponent();

        _usuario = usuario;
        TxtUsuario.Text = "Usuario: " + usuario;
        MainContent.Content = new DashboardView();
        _emailScheduler.Start();
    }

    private void ToggleMenu_Click(object sender, RoutedEventArgs e)
    {
        _menuVisible = !_menuVisible;

        if (_menuVisible)
        {
            MenuColumn.Width = new GridLength(285);
            MenuContent.Visibility = Visibility.Visible;
            MenuFooter.Visibility = Visibility.Visible;
            BtnToggleMenu.Content = "☰";
        }
        else
        {
            MenuColumn.Width = new GridLength(56);
            MenuContent.Visibility = Visibility.Collapsed;
            MenuFooter.Visibility = Visibility.Collapsed;
            BtnToggleMenu.Content = "☰";
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
                "HistorialRespaldoAsistencia" => new HistorialRespaldoAsistenciaView(),
                "Incidencias" => new IncidenciasView(_usuario),
                "Conexiones" => new ConexionesView(),
                "ChecadoresZk" => new ChecadoresZkView(),
                "Catalogos" => new CatalogosView(_usuario),
                "CategoriasEmpleado" => new CategoriasEmpleadoView(),
                "AreaPuestos" => new AreaPuestosView(),
                "Nomina" => new NominaView(_usuario),
                "Bitacora" => new BitacoraView(),
                "ReportesCorreo" => new ReportesCorreoView(),
                "JornadaLaboral" => new JornadaLaboralView(),
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
