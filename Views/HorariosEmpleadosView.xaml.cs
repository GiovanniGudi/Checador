using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClosedXML.Excel;
using Microsoft.Win32;
using ConstructoraAG_WPF_Listo.Models;
using ConstructoraAG_WPF_Listo.Services;

namespace ConstructoraAG_WPF_Listo.Views;

public partial class HorariosEmpleadosView : UserControl
{
    private const int TamanoPagina = 50;
    private readonly string _usuario;
    private readonly HorariosEmpleadoService _service = new();
    private readonly ObservableCollection<EmpleadoHorarioItem> _empleados = new();
    private readonly ObservableCollection<HorarioDiaEdit> _dias = new();
    private readonly ObservableCollection<HistorialHorarioItem> _historial = new();
    private List<EmpleadoHorarioItem> _todosEmpleados = new();
    private int _paginaActual = 1;

    public HorariosEmpleadosView(string usuario)
    {
        InitializeComponent();
        _usuario = usuario;
        GridEmpleados.ItemsSource = _empleados;
        GridDias.ItemsSource = _dias;
        GridHistorial.ItemsSource = _historial;
        CargarDiasPredeterminados();
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        DpVigenteDesde.SelectedDate ??= DateTime.Today;
        await BuscarAsync();
    }

    private void CargarDiasPredeterminados()
    {
        _dias.Clear();
        var nombres = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
        for (var i = 1; i <= 7; i++)
        {
            var laboral = i <= 5;
            _dias.Add(new HorarioDiaEdit
            {
                DiaSemana = i,
                Dia = nombres[i - 1],
                EsLaboral = laboral,
                HoraEntrada = laboral ? "08:00" : "",
                HoraSalida = laboral ? "18:00" : "",
                ToleranciaRetardoMin = 0
            });
        }
    }

    private async void Buscar_Click(object sender, RoutedEventArgs e) => await BuscarAsync();

    private async void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            await BuscarAsync();
    }

    private async Task BuscarAsync()
    {
        try
        {
            _todosEmpleados = await _service.BuscarEmpleadosAsync(TxtBuscar.Text);
            _paginaActual = 1;
            CargarPaginaActual();
        }
        catch (Exception ex)
        {
            MessageBox.Show("No fue posible cargar empleados y horarios.\n\n" + ex.Message,
                "Horarios", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private int TotalPaginas =>
        Math.Max(1, (int)Math.Ceiling(_todosEmpleados.Count / (double)TamanoPagina));

    private void CargarPaginaActual()
    {
        if (_paginaActual < 1) _paginaActual = 1;
        if (_paginaActual > TotalPaginas) _paginaActual = TotalPaginas;

        var pagina = _todosEmpleados
            .Skip((_paginaActual - 1) * TamanoPagina)
            .Take(TamanoPagina)
            .ToList();

        _empleados.Clear();
        foreach (var item in pagina)
            _empleados.Add(item);

        GridEmpleados.SelectedItems.Clear();
        ActualizarPaginacion();
    }

    private void ActualizarPaginacion()
    {
        var total = _todosEmpleados.Count;
        var desde = total == 0 ? 0 : ((_paginaActual - 1) * TamanoPagina) + 1;
        var hasta = total == 0 ? 0 : Math.Min(_paginaActual * TamanoPagina, total);

        TxtPaginacion.Text = $"Registros {desde}-{hasta} de {total}";
        TxtPaginaActual.Text = $"Página {_paginaActual} de {TotalPaginas}";

        var puedeAnterior = total > 0 && _paginaActual > 1;
        var puedeSiguiente = total > 0 && _paginaActual < TotalPaginas;
        BtnPrimeraPagina.IsEnabled = puedeAnterior;
        BtnPaginaAnterior.IsEnabled = puedeAnterior;
        BtnPaginaSiguiente.IsEnabled = puedeSiguiente;
        BtnUltimaPagina.IsEnabled = puedeSiguiente;
    }

    private void PrimeraPagina_Click(object sender, RoutedEventArgs e)
    {
        if (_paginaActual == 1) return;
        _paginaActual = 1;
        CargarPaginaActual();
    }

    private void PaginaAnterior_Click(object sender, RoutedEventArgs e)
    {
        if (_paginaActual <= 1) return;
        _paginaActual--;
        CargarPaginaActual();
    }

    private void PaginaSiguiente_Click(object sender, RoutedEventArgs e)
    {
        if (_paginaActual >= TotalPaginas) return;
        _paginaActual++;
        CargarPaginaActual();
    }

    private void UltimaPagina_Click(object sender, RoutedEventArgs e)
    {
        if (_paginaActual == TotalPaginas) return;
        _paginaActual = TotalPaginas;
        CargarPaginaActual();
    }

    private async void GridEmpleados_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = GridEmpleados.SelectedItem as EmpleadoHorarioItem;
        if (selected is null)
        {
            TxtEmpleadoSeleccionado.Text = GridEmpleados.SelectedItems.Count > 1
                ? $"{GridEmpleados.SelectedItems.Count} empleados seleccionados"
                : "Ninguno";
            TxtUbicacionSeleccionada.Text = "";
            return;
        }

        TxtEmpleadoSeleccionado.Text =
            $"{selected.NumeroEmpleado} - {selected.NombreCompleto}  |  PIN: {selected.Pin}";
        TxtUbicacionSeleccionada.Text = "Ubicación: " + selected.Ubicacion;

        try
        {
            var history = await _service.ObtenerHistorialAsync(selected.NumeroEmpleado);
            _historial.Clear();
            foreach (var item in history)
                _historial.Add(item);

            var vigente = await _service.ObtenerHorarioVigenteAsync(selected.NumeroEmpleado, DateTime.Today);
            if (vigente is not null)
            {
                TxtNombreHorario.Text = vigente.NombreHorario;
                _dias.Clear();
                foreach (var dia in vigente.Dias)
                    _dias.Add(dia);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("No fue posible cargar el historial del empleado.\n\n" + ex.Message,
                "Horarios", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void Guardar_Click(object sender, RoutedEventArgs e)
    {
        var selected = GridEmpleados.SelectedItems.Cast<EmpleadoHorarioItem>().ToList();
        if (selected.Count == 0)
        {
            MessageBox.Show("Seleccione uno o varios empleados.", "Horarios",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var desde = DpVigenteDesde.SelectedDate?.Date ?? DateTime.Today;

        try
        {
            await _service.AsignarHorarioAsync(
                selected, TxtNombreHorario.Text, _dias.ToList(), desde, _usuario);

            MessageBox.Show(
                $"Horario guardado para {selected.Count} empleado(s), vigente desde {desde:dd/MM/yyyy}.",
                "Horarios", MessageBoxButton.OK, MessageBoxImage.Information);

            await BuscarAsync();
            if (selected.Count == 1)
            {
                var history = await _service.ObtenerHistorialAsync(selected[0].NumeroEmpleado);
                _historial.Clear();
                foreach (var item in history)
                    _historial.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("No fue posible guardar el horario.\n\n" + ex.Message,
                "Horarios", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void ExportarExcel_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "Excel (*.xlsx)|*.xlsx",
            FileName = $"Horarios_Empleados_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        };
        if (dialog.ShowDialog() != true) return;

        try
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Empleados");
            var headers = new[] { "Número empleado", "PIN", "Nombre", "Ubicación", "Estado",
                                  "Horario actual", "Vigente desde", "Vigente hasta" };
            for (var i = 0; i < headers.Length; i++) ws.Cell(1, i + 1).Value = headers[i];

            var row = 2;
            foreach (var eItem in _todosEmpleados)
            {
                ws.Cell(row, 1).Value = eItem.NumeroEmpleado;
                ws.Cell(row, 2).Value = eItem.Pin;
                ws.Cell(row, 3).Value = eItem.NombreCompleto;
                ws.Cell(row, 4).Value = eItem.Ubicacion;
                ws.Cell(row, 5).Value = eItem.Estado;
                ws.Cell(row, 6).Value = eItem.HorarioActual;
                if (eItem.VigenteDesde.HasValue) ws.Cell(row, 7).Value = eItem.VigenteDesde.Value;
                if (eItem.VigenteHasta.HasValue) ws.Cell(row, 8).Value = eItem.VigenteHasta.Value;
                row++;
            }
            ws.Columns().AdjustToContents();

            var detail = wb.Worksheets.Add("Configuraciones");
            var detailHeaders = new[] { "Número empleado", "PIN", "Nombre", "Horario", "Vigente desde",
                                        "Día", "Laboral/Descanso", "Entrada", "Salida", "Tolerancia retardo" };
            for (var i = 0; i < detailHeaders.Length; i++) detail.Cell(1, i + 1).Value = detailHeaders[i];

            row = 2;
            foreach (var eItem in _todosEmpleados)
            {
                var vigente = await _service.ObtenerHorarioVigenteAsync(eItem.NumeroEmpleado, DateTime.Today);
                if (vigente is null) continue;
                foreach (var dia in vigente.Dias)
                {
                    detail.Cell(row, 1).Value = eItem.NumeroEmpleado;
                    detail.Cell(row, 2).Value = eItem.Pin;
                    detail.Cell(row, 3).Value = eItem.NombreCompleto;
                    detail.Cell(row, 4).Value = vigente.NombreHorario;
                    detail.Cell(row, 5).Value = vigente.VigenteDesde;
                    detail.Cell(row, 6).Value = dia.Dia;
                    detail.Cell(row, 7).Value = dia.EsLaboral ? "LABORAL" : "DESCANSO";
                    detail.Cell(row, 8).Value = dia.HoraEntrada;
                    detail.Cell(row, 9).Value = dia.HoraSalida;
                    detail.Cell(row, 10).Value = dia.ToleranciaRetardoMin;
                    row++;
                }
            }
            detail.Columns().AdjustToContents();

            wb.SaveAs(dialog.FileName);
            MessageBox.Show("Exportación Excel generada correctamente.", "Horarios");
        }
        catch (Exception ex)
        {
            MessageBox.Show("No fue posible exportar a Excel.\n\n" + ex.Message, "Horarios",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void ExportarCsv_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"Horarios_Empleados_{DateTime.Now:yyyyMMdd_HHmm}.csv"
        };
        if (dialog.ShowDialog() != true) return;

        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("NumeroEmpleado,PIN,Nombre,Ubicacion,Horario,VigenteDesde,Dia,Tipo,Entrada,Salida,ToleranciaRetardoMin");

            foreach (var eItem in _todosEmpleados)
            {
                var vigente = await _service.ObtenerHorarioVigenteAsync(eItem.NumeroEmpleado, DateTime.Today);
                if (vigente is null)
                {
                    sb.AppendLine(string.Join(",", new[]
                    {
                        Csv(eItem.NumeroEmpleado), Csv(eItem.Pin), Csv(eItem.NombreCompleto), Csv(eItem.Ubicacion),
                        Csv("SIN HORARIO"), "", "", "", "", "", ""
                    }));
                    continue;
                }

                foreach (var dia in vigente.Dias)
                {
                    sb.AppendLine(string.Join(",", new[]
                    {
                        Csv(eItem.NumeroEmpleado), Csv(eItem.Pin), Csv(eItem.NombreCompleto), Csv(eItem.Ubicacion),
                        Csv(vigente.NombreHorario), vigente.VigenteDesde.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Csv(dia.Dia), Csv(dia.EsLaboral ? "LABORAL" : "DESCANSO"),
                        Csv(dia.HoraEntrada), Csv(dia.HoraSalida), dia.ToleranciaRetardoMin.ToString()
                    }));
                }
            }

            await File.WriteAllTextAsync(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
            MessageBox.Show("Exportación CSV generada correctamente.", "Horarios");
        }
        catch (Exception ex)
        {
            MessageBox.Show("No fue posible exportar a CSV.\n\n" + ex.Message, "Horarios",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static string Csv(string? value)
    {
        value ??= "";
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
