namespace ConstructoraAG_WPF_Listo.Models;

public sealed class HorarioReglaEmpleadoFecha
{
    public int IdEmpleado { get; set; }
    public DateTime Fecha { get; set; }
    public int IdHorario { get; set; }
    public string NombreHorario { get; set; } = "";
    public bool EsLaboral { get; set; }
    public TimeSpan? HoraEntrada { get; set; }
    public TimeSpan? HoraSalida { get; set; }
    public int ToleranciaRetardoMin { get; set; }
}
