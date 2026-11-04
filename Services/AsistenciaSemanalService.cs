using ConstructoraAG_WPF_Listo.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConstructoraAG_WPF_Listo.Services;

public class AsistenciaSemanalService
{
    private readonly AsistenciaService _asistencia = new();
    private readonly JornadaLaboralService _jornada = new();
    private readonly EmpleadoService _empleados = new();
    private readonly DatabaseService _db = new();
    private readonly HorariosEmpleadoService _horarios = new();

    public async Task<List<AsistenciaSemanalRowModel>> BuscarPeriodoAsync(DateTime fechaInicial, DateTime fechaFinal)
    {
        fechaInicial = fechaInicial.Date;
        fechaFinal = fechaFinal.Date;
        if (fechaFinal < fechaInicial)
            throw new ArgumentException("La fecha final no puede ser menor que la fecha inicial.");

        var registros = await _asistencia.BuscarAsync(
            fechaInicial,
            fechaFinal,
            "",
            "Todos",
            "",
            "",
            false,
            false,
            false,
            false,
            exigirTodasBasesActivas: false,
            consolidarEntreBases: true);

        var conexionesPorPin = await ObtenerConexionesPredominantesAsync(fechaInicial, fechaFinal);
        var cfg = _jornada.Cargar();
        var hoy = DateTime.Today;
        var empleadosAdmin = await _empleados.BuscarAsync("");

        var reglasHorarioPeriodo = await _horarios.ObtenerReglasPeriodoAsync(fechaInicial, fechaFinal);

        var empleadosPorPin = empleadosAdmin
            .Where(e => !string.IsNullOrWhiteSpace(NormalizarPin(e.PinEmpleado)))
            .GroupBy(e => NormalizarPin(e.PinEmpleado), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        return registros
            .Where(x => !string.IsNullOrWhiteSpace(NormalizarPin(x.PinChecador)))
            .GroupBy(x => NormalizarPin(x.PinChecador), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Any(TieneChecadaReal))
            .Select(g =>
            {
                empleadosPorPin.TryGetValue(g.Key, out var empleadoAdmin);
                var fila = ConstruirFila(
                    g.Key,
                    g.ToList(),
                    empleadoAdmin,
                    fechaInicial,
                    fechaFinal,
                    cfg,
                    hoy,
                    reglasHorarioPeriodo);

                if (conexionesPorPin.TryGetValue(g.Key, out var predominante))
                {
                    fila.ConexionPredominante = predominante.Nombre;
                    fila.DetalleConexiones = predominante.Detalle;
                }
                return fila;
            })
            .OrderBy(x => string.IsNullOrWhiteSpace(x.NombreCompleto) ? x.PinChecador : x.NombreCompleto,
                StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    private async Task<Dictionary<string, (string Nombre, string Detalle)>>
        ObtenerConexionesPredominantesAsync(DateTime inicio, DateTime fin)
    {
        const string sql = @"
SELECT ISNULL(NULLIF(LTRIM(RTRIM(CONVERT(varchar(50), h.emp_pin))), ''),
              CONVERT(varchar(50), p.employee_id)) AS PinChecador,
       CONVERT(date, p.punch_time) AS Dia,
       COUNT_BIG(*) AS Marcaciones
FROM dbo.att_punches p
LEFT JOIN dbo.hr_employee h ON h.id = p.employee_id
WHERE p.punch_time >= @inicio AND p.punch_time < @fin
GROUP BY ISNULL(NULLIF(LTRIM(RTRIM(CONVERT(varchar(50), h.emp_pin))), ''),
                CONVERT(varchar(50), p.employee_id)), CONVERT(date, p.punch_time)";

        var datos = await _db.QueryChecadoresActivosCompletosReadOnlyAsync(sql,
            new SqlParameter("@inicio", inicio.Date),
            new SqlParameter("@fin", fin.Date.AddDays(1)));

        var grupos = datos.Rows.Cast<DataRow>()
            .Select(r => new
            {
                Pin = NormalizarPin(r["PinChecador"]?.ToString()),
                IdConexion = (r["ConexionIdOrigen"]?.ToString() ?? "").Trim(),
                Conexion = (r["ConexionOrigen"]?.ToString() ?? "").Trim(),
                Base = (r["BaseDatosOrigen"]?.ToString() ?? "").Trim(),
                Ubicacion = (r["UbicacionOrigen"]?.ToString() ?? "").Trim(),
                Dia = Convert.ToDateTime(r["Dia"]).Date,
                Marcaciones = Convert.ToInt64(r["Marcaciones"])
            })
            .Where(x => x.Pin.Length > 0)
            .GroupBy(x => x.Pin, StringComparer.OrdinalIgnoreCase);

        var resultado = new Dictionary<string, (string Nombre, string Detalle)>(StringComparer.OrdinalIgnoreCase);
        foreach (var porPin in grupos)
        {
            var sedes = porPin
                .GroupBy(x => x.IdConexion.Length > 0 ? x.IdConexion : $"{x.Conexion}|{x.Base}", StringComparer.OrdinalIgnoreCase)
                .Select(g => new
                {
                    Nombre = g.First().Conexion.Length > 0 ? g.First().Conexion : g.First().Base,
                    Base = g.First().Base,
                    Ubicacion = g.First().Ubicacion,
                    IdConexion = g.First().IdConexion,
                    Dias = g.Select(x => x.Dia).Distinct().Count(),
                    Marcaciones = g.Sum(x => x.Marcaciones)
                })
                .OrderByDescending(x => x.Dias)
                .ThenBy(x => x.Nombre, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (sedes.Count == 0) continue;

            var nombresRepetidos = sedes
                .GroupBy(x => x.Nombre, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            string NombreVisible(string nombre, string bd, string id) =>
                nombresRepetidos.Contains(nombre)
                    ? $"{nombre} [{bd}; {id}]" : nombre;

            int maxDias = sedes[0].Dias;
            var empatadas = sedes.Where(x => x.Dias == maxDias).ToList();
            string nombre = empatadas.Count == 1
                ? NombreVisible(empatadas[0].Nombre, empatadas[0].Base, empatadas[0].IdConexion)
                : "EMPATE: " + string.Join(" / ", empatadas.Select(x =>
                    NombreVisible(x.Nombre, x.Base, x.IdConexion)));

            string detalle = string.Join("\n", sedes.Select(x =>
                $"{NombreVisible(x.Nombre, x.Base, x.IdConexion)}{(x.Ubicacion.Length > 0 ? $" ({x.Ubicacion})" : "")}: " +
                $"{x.Dias} día(s), {x.Marcaciones} checada(s)"));

            resultado[porPin.Key] = (nombre, detalle);
        }
        return resultado;
    }

    private static string NormalizarNombreComparacion(string nombre)
    {
        var descompuesto = nombre.Trim().Normalize(System.Text.NormalizationForm.FormD);
        var limpio = new System.Text.StringBuilder();
        bool espacioAnterior = false;
        foreach (char c in descompuesto)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.NonSpacingMark)
                continue;
            if (char.IsWhiteSpace(c))
            {
                if (!espacioAnterior && limpio.Length > 0) limpio.Append(' ');
                espacioAnterior = true;
            }
            else
            {
                limpio.Append(char.ToUpperInvariant(c));
                espacioAnterior = false;
            }
        }
        return limpio.ToString().Trim();
    }

    public static DateTime ObtenerLunes(DateTime fecha)
    {
        fecha = fecha.Date;
        int diff = ((int)fecha.DayOfWeek + 6) % 7;
        return fecha.AddDays(-diff);
    }

    private static AsistenciaSemanalRowModel ConstruirFila(
        string pin,
        List<AsistenciaModel> registros,
        EmpleadoModel? empleadoAdmin,
        DateTime fechaInicial,
        DateTime fechaFinal,
        JornadaLaboralConfig cfg,
        DateTime hoy,
        IReadOnlyDictionary<(int IdEmpleado, DateTime Fecha), HorarioReglaEmpleadoFecha> reglasHorarioPeriodo)
    {
        var conDatosSistema = registros.FirstOrDefault(r => r.IdEmpleadoSistema > 0) ?? registros.First();

        var nombresChecador = registros.Select(r => (r.NombreChecador ?? "").Trim())
            .Where(n => n.Length > 0)
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        string nombreChecadorPersona = string.Join(" / ", nombresChecador);
        string nombreTablaEmpleados = (empleadoAdmin?.NombreCompleto ?? "").Trim();

        string validacion = nombresChecador.Count == 0 || nombreTablaEmpleados.Length == 0
            ? "SIN DATOS PARA COMPARAR"
            : nombresChecador.Count > 1
                ? "REVISAR: VARIOS NOMBRES EN CHECADOR"
                : NormalizarNombreComparacion(nombresChecador[0]) == NormalizarNombreComparacion(nombreTablaEmpleados)
                    ? "COINCIDEN"
                    : "REVISAR: NOMBRES DIFERENTES";

        var fila = new AsistenciaSemanalRowModel
        {
            PinChecador = pin,
            NombrePersonaChecador = nombreChecadorPersona.Length > 0 ? nombreChecadorPersona : "Sin nombre en checador",
            NombreEmpleadoTabla = nombreTablaEmpleados.Length > 0 ? nombreTablaEmpleados : "Sin registro en Empleados",
            ValidacionNombres = validacion,
            NombreChecador = UnirDistintos(registros.SelectMany(r => Separar(r.Terminal))),
            NumeroEmpleado = !string.IsNullOrWhiteSpace(empleadoAdmin?.PinEmpleado)
                ? empleadoAdmin!.PinEmpleado.Trim()
                : conDatosSistema.PinEmpleadoSistema > 0 ? conDatosSistema.PinEmpleadoSistema.ToString() : pin,
            NombreCompleto = !string.IsNullOrWhiteSpace(empleadoAdmin?.NombreCompleto)
                ? empleadoAdmin!.NombreCompleto.Trim()
                : !string.IsNullOrWhiteSpace(conDatosSistema.NombreCompletoSistema)
                    ? conDatosSistema.NombreCompletoSistema
                    : conDatosSistema.NombreChecador,
            Ubicacion = !string.IsNullOrWhiteSpace(empleadoAdmin?.Ubicacion)
                ? empleadoAdmin!.Ubicacion.Trim()
                : conDatosSistema.Ubicacion ?? "",
            Area = !string.IsNullOrWhiteSpace(empleadoAdmin?.Area)
                ? empleadoAdmin!.Area.Trim()
                : conDatosSistema.Area ?? "",
            Puesto = !string.IsNullOrWhiteSpace(empleadoAdmin?.Puesto)
                ? empleadoAdmin!.Puesto.Trim()
                : conDatosSistema.Puesto ?? "",
            Fotografia = empleadoAdmin?.Fotografia
        };

        var dias = new List<DiaAsistenciaSemanalModel>();
        for (var fecha = fechaInicial.Date; fecha <= fechaFinal.Date; fecha = fecha.AddDays(1))
        {
            var delDia = registros
                .Where(r => DateTime.TryParse(r.Fecha, out var f) && f.Date == fecha.Date)
                .ToList();

            HorarioReglaEmpleadoFecha? reglaHorario = null;
            if (empleadoAdmin?.IdEmpleado > 0)
                reglasHorarioPeriodo.TryGetValue((empleadoAdmin.IdEmpleado, fecha.Date), out reglaHorario);

            dias.Add(ConstruirDia(fecha, delDia, cfg, hoy, true, reglaHorario));
        }

        fila.DiasPeriodo = dias;
        fila.Lunes = dias.FirstOrDefault(d => d.Fecha.DayOfWeek == DayOfWeek.Monday) ?? new();
        fila.Martes = dias.FirstOrDefault(d => d.Fecha.DayOfWeek == DayOfWeek.Tuesday) ?? new();
        fila.Miercoles = dias.FirstOrDefault(d => d.Fecha.DayOfWeek == DayOfWeek.Wednesday) ?? new();
        fila.Jueves = dias.FirstOrDefault(d => d.Fecha.DayOfWeek == DayOfWeek.Thursday) ?? new();
        fila.Viernes = dias.FirstOrDefault(d => d.Fecha.DayOfWeek == DayOfWeek.Friday) ?? new();
        fila.Sabado = dias.FirstOrDefault(d => d.Fecha.DayOfWeek == DayOfWeek.Saturday) ?? new();
        fila.Domingo = dias.FirstOrDefault(d => d.Fecha.DayOfWeek == DayOfWeek.Sunday) ?? new();
        fila.FaltasDetalle = dias.Where(d => d.EsFalta).ToList();
        fila.Faltas = fila.FaltasDetalle.Count;
        fila.DiasLaboralesEvaluables = dias.Count(d => d.Laboral && !d.EsFuturo);
        fila.DiasConAsistencia = dias.Count(d => d.Laboral && !d.EsFuturo && d.TieneRegistro);
        return fila;
    }

    private static DiaAsistenciaSemanalModel ConstruirDia(
        DateTime fecha,
        List<AsistenciaModel> registros,
        JornadaLaboralConfig cfg,
        DateTime hoy,
        bool enPeriodo,
        HorarioReglaEmpleadoFecha? reglaHorario = null)
    {
        if (!enPeriodo)
        {
            return new DiaAsistenciaSemanalModel
            {
                Fecha = fecha.Date,
                Dia = NombreDia(fecha.DayOfWeek),
                EnPeriodo = false,
                Laboral = false,
                EsFuturo = fecha.Date > hoy,
                EsFalta = false,
                TieneRegistro = false,
                Estado = "Fuera de periodo"
            };
        }

        var referenciaHorario = registros.FirstOrDefault();

        bool laboral;
        if (reglaHorario != null)
            laboral = reglaHorario.EsLaboral;
        else
            laboral = referenciaHorario?.UsaHorarioAsignado == true
                ? referenciaHorario.EsDiaLaboralProgramado
                : EsLaboral(cfg, fecha.DayOfWeek);

        bool futuro = fecha.Date > hoy;

        var reales = registros
            .Where(r => !string.IsNullOrWhiteSpace(r.HoraEntrada) || !string.IsNullOrWhiteSpace(r.HoraSalida))
            .ToList();

        bool tiene = reales.Count > 0;

        string entrada;
        string salida;

        // Si existe un horario individual vigente, únicamente cambia la selección
        // de las marcaciones oficiales. Los registros originales y todos los demás
        // cálculos permanecen intactos.
        if (reglaHorario?.EsLaboral == true &&
            reglaHorario.HoraEntrada.HasValue &&
            reglaHorario.HoraSalida.HasValue)
        {
            (entrada, salida) = SeleccionarChecadasSegunHorario(
                fecha,
                reales,
                reglaHorario.HoraEntrada.Value,
                reglaHorario.HoraSalida.Value);
        }
        else
        {
            // Compatibilidad total: sin horario asignado se conserva exactamente
            // la lógica heredada del sistema.
            entrada = reales
                .Select(r => NormalizarHora(r.HoraEntrada))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .OrderBy(x => x, StringComparer.Ordinal)
                .FirstOrDefault() ?? "";

            salida = reales
                .Select(r => NormalizarHora(r.HoraSalida))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .OrderBy(x => x, StringComparer.Ordinal)
                .LastOrDefault() ?? "";
        }

        string estado;
        if (futuro)
            estado = "Día futuro";
        else if (!laboral && !tiene)
            estado = "No laborable";
        else if (!tiene)
            estado = "Sin checadas";
        else if (string.IsNullOrWhiteSpace(entrada) ^ string.IsNullOrWhiteSpace(salida))
            estado = "Única checada";
        else if (!laboral)
            estado = "Asistencia en día no laborable";
        else
            estado = "Asistencia";

        var referencia = registros.FirstOrDefault();
        if (referencia != null && !string.IsNullOrWhiteSpace(referencia.EstatusJornada) && !futuro)
            estado = referencia.EstatusJornada;

        return new DiaAsistenciaSemanalModel
        {
            Fecha = fecha.Date,
            Dia = NombreDia(fecha.DayOfWeek),
            EnPeriodo = true,
            Laboral = laboral,
            EsFuturo = futuro,
            EsFalta = laboral && !futuro && !tiene,
            TieneRegistro = tiene,
            Entrada = entrada,
            Salida = salida,
            Estado = estado,
            Terminales = UnirDistintos(reales.SelectMany(r => Separar(r.Terminal))),
            HorarioAsignado = reglaHorario?.NombreHorario ?? referencia?.HorarioAsignado ?? "",
            EntradaProgramada = reglaHorario?.EsLaboral == true && reglaHorario.HoraEntrada.HasValue
                ? reglaHorario.HoraEntrada.Value.ToString(@"hh\:mm")
                : referencia?.HoraEntradaProgramada ?? "",
            SalidaProgramada = reglaHorario?.EsLaboral == true && reglaHorario.HoraSalida.HasValue
                ? reglaHorario.HoraSalida.Value.ToString(@"hh\:mm")
                : referencia?.HoraSalidaProgramada ?? "",
            RetardoMinutos = referencia?.RetardoMinutos ?? 0,
            SalidaAnticipadaMinutos = referencia?.SalidaAnticipadaMinutos ?? 0
        };
    }

    /// <summary>
    /// Selecciona las marcaciones oficiales usando el horario asignado.
    ///
    /// Entrada:
    /// - reúne las horas disponibles del día;
    /// - elige la marcación con menor distancia respecto a la entrada programada.
    ///
    /// Salida:
    /// - nunca considera una marcación anterior o igual a la entrada elegida;
    /// - para mantener compatibilidad con la lógica histórica de cierre, toma
    ///   la última marcación posterior a la entrada.
    ///
    /// No modifica ni elimina ninguna marcación original.
    /// </summary>
    private static (string Entrada, string Salida) SeleccionarChecadasSegunHorario(
        DateTime fecha,
        IReadOnlyCollection<AsistenciaModel> registros,
        TimeSpan horaEntradaProgramada,
        TimeSpan horaSalidaProgramada)
    {
        var marcaciones = new List<DateTime>();

        foreach (var r in registros)
        {
            AgregarMarcacion(marcaciones, fecha, r.HoraEntrada);
            AgregarMarcacion(marcaciones, fecha, r.HoraSalida);
        }

        marcaciones = marcaciones
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        if (marcaciones.Count == 0)
            return ("", "");

        var entradaEsperada = fecha.Date + horaEntradaProgramada;
        var salidaEsperada = fecha.Date + horaSalidaProgramada;

        // Soporte de turnos nocturnos a nivel de regla. En el modelo actual
        // las marcaciones entregadas a este método pertenecen al día evaluado;
        // por eso no se altera la consulta de origen ni se mezclan días.
        if (horaSalidaProgramada < horaEntradaProgramada)
            salidaEsperada = salidaEsperada.AddDays(1);

        var entradaSeleccionada = marcaciones
            .OrderBy(x => Math.Abs((x - entradaEsperada).TotalMinutes))
            // En empate se conserva la marcación anterior, que es la opción
            // menos agresiva respecto a la lógica histórica.
            .ThenBy(x => x)
            .First();

        var posteriores = marcaciones
            .Where(x => x > entradaSeleccionada)
            .OrderBy(x => x)
            .ToList();

        // Si sólo existe una marcación válida alrededor del turno, se conserva
        // como entrada y no se inventa una salida.
        if (posteriores.Count == 0)
            return (
                entradaSeleccionada.ToString("HH:mm"),
                "");

        // Mantener el cierre histórico (última marcación), pero ahora únicamente
        // después de la entrada inteligente. Esto evita usar como salida una
        // marcación previa o ajena al inicio oficial del turno.
        var salidaSeleccionada = posteriores.Last();

        return (
            entradaSeleccionada.ToString("HH:mm"),
            salidaSeleccionada.ToString("HH:mm"));
    }

    private static void AgregarMarcacion(
        ICollection<DateTime> destino,
        DateTime fecha,
        string? hora)
    {
        if (string.IsNullOrWhiteSpace(hora))
            return;

        if (!TimeSpan.TryParse(hora.Trim(), out var valor))
            return;

        destino.Add(fecha.Date + valor);
    }

    private static bool TieneChecadaReal(AsistenciaModel registro) =>
        !string.IsNullOrWhiteSpace(registro.HoraEntrada) ||
        !string.IsNullOrWhiteSpace(registro.HoraSalida);

    private static string NormalizarHora(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "";
        if (TimeSpan.TryParse(value, out var t)) return t.ToString(@"hh\:mm");
        return value.Trim();
    }

    private static bool EsLaboral(JornadaLaboralConfig cfg, DayOfWeek dow)
    {
        var nombre = NombreDia(dow);
        return cfg.Dias.FirstOrDefault(d => d.Dia.Equals(nombre, StringComparison.OrdinalIgnoreCase))?.Laboral ?? false;
    }

    private static string NombreDia(DayOfWeek d) => d switch
    {
        DayOfWeek.Monday => "Lunes",
        DayOfWeek.Tuesday => "Martes",
        DayOfWeek.Wednesday => "Miércoles",
        DayOfWeek.Thursday => "Jueves",
        DayOfWeek.Friday => "Viernes",
        DayOfWeek.Saturday => "Sábado",
        _ => "Domingo"
    };

    private static string NormalizarPin(string? pin)
    {
        pin = (pin ?? "").Trim();
        return long.TryParse(pin, out var n) ? n.ToString() : pin;
    }

    private static IEnumerable<string> Separar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor)) yield break;
        foreach (var item in valor.Split(new[] { " / " }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            yield return item;
    }

    private static string UnirDistintos(IEnumerable<string> valores) =>
        string.Join(" / ", valores.Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase));
}
