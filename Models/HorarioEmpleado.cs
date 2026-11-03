using System.Collections.ObjectModel;

namespace ConstructoraAG_WPF_Listo.Models;

public sealed class EmpleadoHorarioItem
{
    public string NumeroEmpleado { get; set; } = "";
    public string Pin { get; set; } = "";
    public string NombreCompleto { get; set; } = "";
    public string Ubicacion { get; set; } = "";
    public string Estado { get; set; } = "";
    public string HorarioActual { get; set; } = "SIN HORARIO";
    public DateTime? VigenteDesde { get; set; }
    public DateTime? VigenteHasta { get; set; }
}

public sealed class HorarioDiaEdit
{
    public int DiaSemana { get; set; } // 1=Lunes ... 7=Domingo
    public string Dia { get; set; } = "";
    public bool EsLaboral { get; set; }
    public string HoraEntrada { get; set; } = "08:00";
    public string HoraSalida { get; set; } = "18:00";
    public int ToleranciaRetardoMin { get; set; }

    public TimeSpan? HoraEntradaTime =>
        EsLaboral && TimeSpan.TryParse(HoraEntrada, out var value) ? value : null;

    public TimeSpan? HoraSalidaTime =>
        EsLaboral && TimeSpan.TryParse(HoraSalida, out var value) ? value : null;
}

public sealed class HorarioVigente
{
    public long IdAsignacion { get; set; }
    public int IdHorario { get; set; }
    public string NumeroEmpleado { get; set; } = "";
    public string PinChecador { get; set; } = "";
    public string NombreHorario { get; set; } = "";
    public DateTime VigenteDesde { get; set; }
    public DateTime? VigenteHasta { get; set; }
    public List<HorarioDiaEdit> Dias { get; set; } = new();
}

public sealed class ReglaHorarioDia
{
    public int IdHorario { get; set; }
    public string NombreHorario { get; set; } = "";
    public DateTime Fecha { get; set; }
    public bool EsLaboral { get; set; }
    public TimeSpan? HoraEntrada { get; set; }
    public TimeSpan? HoraSalida { get; set; }
    public int ToleranciaRetardoMin { get; set; }
}

public sealed class ResultadoHorarioReporte
{
    public bool UsaHorarioConfigurado { get; set; }
    public bool EsDescanso { get; set; }
    public bool EsFalta { get; set; }
    public int RetardoMinutos { get; set; }
    public int SalidaAnticipadaMinutos { get; set; }
    public int DiferenciaJornadaMinutos { get; set; }
    public DateTime? EntradaEsperada { get; set; }
    public DateTime? SalidaEsperada { get; set; }
    public string Estatus { get; set; } = "";
}

public sealed class HistorialHorarioItem
{
    public long IdAsignacion { get; set; }
    public string NumeroEmpleado { get; set; } = "";
    public string PinChecador { get; set; } = "";
    public string NombreHorario { get; set; } = "";
    public DateTime VigenteDesde { get; set; }
    public DateTime? VigenteHasta { get; set; }
    public DateTime FechaCambio { get; set; }
    public string UsuarioCambio { get; set; } = "";
}
