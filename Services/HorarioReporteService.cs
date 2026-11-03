using ConstructoraAG_WPF_Listo.Models;

namespace ConstructoraAG_WPF_Listo.Services;

/// <summary>
/// Capa opcional para reportes.
/// Regla fundamental: si Get/Calcular devuelve UsaHorarioConfigurado=false,
/// el reporte DEBE ejecutar exactamente su lógica heredada.
/// </summary>
public sealed class HorarioReporteService
{
    private readonly HorariosEmpleadoService _horarios = new();

    public async Task<ResultadoHorarioReporte> CalcularAsync(
        string claveEmpleado,
        DateTime fecha,
        DateTime? entradaReal,
        DateTime? salidaReal)
    {
        var regla = await _horarios.ObtenerReglaDiaAsync(claveEmpleado, fecha);

        if (regla is null)
        {
            return new ResultadoHorarioReporte
            {
                UsaHorarioConfigurado = false,
                Estatus = "SIN HORARIO CONFIGURADO"
            };
        }

        if (!regla.EsLaboral)
        {
            return new ResultadoHorarioReporte
            {
                UsaHorarioConfigurado = true,
                EsDescanso = true,
                EsFalta = false,
                Estatus = entradaReal.HasValue ? "REGISTRO EN DÍA DE DESCANSO" : "DESCANSO"
            };
        }

        var expectedIn = fecha.Date + regla.HoraEntrada!.Value;
        var expectedOut = fecha.Date + regla.HoraSalida!.Value;
        if (regla.HoraSalida.Value < regla.HoraEntrada.Value)
            expectedOut = expectedOut.AddDays(1);

        var lateLimit = expectedIn.AddMinutes(Math.Max(0, regla.ToleranciaRetardoMin));

        if (!entradaReal.HasValue)
        {
            return new ResultadoHorarioReporte
            {
                UsaHorarioConfigurado = true,
                EsFalta = true,
                EntradaEsperada = expectedIn,
                SalidaEsperada = expectedOut,
                DiferenciaJornadaMinutos = (int)Math.Round((expectedOut - expectedIn).TotalMinutes),
                Estatus = "FALTA"
            };
        }

        var late = Math.Max(0, (int)Math.Floor((entradaReal.Value - lateLimit).TotalMinutes));
        var early = salidaReal.HasValue
            ? Math.Max(0, (int)Math.Ceiling((expectedOut - salidaReal.Value).TotalMinutes))
            : 0;

        var expectedMinutes = Math.Max(0, (int)Math.Round((expectedOut - expectedIn).TotalMinutes));
        var actualMinutes = salidaReal.HasValue
            ? Math.Max(0, (int)Math.Round((salidaReal.Value - entradaReal.Value).TotalMinutes))
            : 0;

        var status = !salidaReal.HasValue ? "SIN SALIDA"
            : late > 0 && early > 0 ? "RETARDO / SALIDA ANTICIPADA"
            : late > 0 ? "RETARDO"
            : early > 0 ? "SALIDA ANTICIPADA"
            : "EN HORARIO";

        return new ResultadoHorarioReporte
        {
            UsaHorarioConfigurado = true,
            EsFalta = false,
            EsDescanso = false,
            RetardoMinutos = late,
            SalidaAnticipadaMinutos = early,
            DiferenciaJornadaMinutos = actualMinutes - expectedMinutes,
            EntradaEsperada = expectedIn,
            SalidaEsperada = expectedOut,
            Estatus = status
        };
    }
}
