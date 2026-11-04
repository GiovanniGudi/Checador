using System.IO;
using System.Windows.Media.Imaging;

namespace ConstructoraAG_WPF_Listo.Models;

public class DiaAsistenciaSemanalModel
{
    public DateTime Fecha { get; set; }
    public string Dia { get; set; } = "";
    public bool EnPeriodo { get; set; } = true;
    public bool Laboral { get; set; }
    public bool EsFuturo { get; set; }
    public bool EsFalta { get; set; }
    public bool TieneRegistro { get; set; }
    public string Entrada { get; set; } = "";
    public string Salida { get; set; } = "";
    public string Estado { get; set; } = "";
    public string Terminales { get; set; } = "";
    public string HorarioAsignado { get; set; } = "";
    public string EntradaProgramada { get; set; } = "";
    public string SalidaProgramada { get; set; } = "";
    public int RetardoMinutos { get; set; }
    public int SalidaAnticipadaMinutos { get; set; }
    public string LaboralTexto => Laboral ? "Sí" : "No";

    public string Texto
    {
        get
        {
            var fecha = Fecha.ToString("dd/MM");
            if (!EnPeriodo) return $"{fecha}\nFuera de periodo";
            if (EsFuturo) return $"{fecha}\nDía futuro";
            if (!Laboral && !TieneRegistro) return $"{fecha}\nNo laborable";
            if (EsFalta) return $"{fecha}\nSin checadas\nFalta";
            if (!TieneRegistro) return $"{fecha}\nSin checadas\nNo laborable";

            if (Estado == "Única checada")
            {
                var hora = !string.IsNullOrWhiteSpace(Entrada) ? Entrada : Salida;
                return $"{fecha}\nChecada: {hora}\nÚnica checada";
            }

            var ent = string.IsNullOrWhiteSpace(Entrada) ? "--:--" : Entrada;
            var sal = string.IsNullOrWhiteSpace(Salida) ? "--:--" : Salida;
            return $"{fecha}\nEntrada: {ent}\nSalida: {sal}\n{Estado}";
        }
    }
}

public class AsistenciaSemanalRowModel
{
    public string PinChecador { get; set; } = "";
    public string NombreChecador { get; set; } = "";
    public string NumeroEmpleado { get; set; } = "";
    public string NombrePersonaChecador { get; set; } = "";
    public string NombreEmpleadoTabla { get; set; } = "";
    public string ValidacionNombres { get; set; } = "";
    public string NombreCompleto { get; set; } = "";
    public string Ubicacion { get; set; } = "";
    public string ConexionPredominante { get; set; } = "Sin checadas por conexión";
    public string DetalleConexiones { get; set; } = "";
    public string Area { get; set; } = "";
    public string Puesto { get; set; } = "";
    public byte[]? Fotografia { get; set; }

    public BitmapImage? FotografiaImagen
    {
        get
        {
            if (Fotografia == null || Fotografia.Length == 0) return null;
            try
            {
                using var ms = new MemoryStream(Fotografia);
                var img = new BitmapImage();
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
                img.Freeze();
                return img;
            }
            catch { return null; }
        }
    }

    public DiaAsistenciaSemanalModel Lunes { get; set; } = new();
    public DiaAsistenciaSemanalModel Martes { get; set; } = new();
    public DiaAsistenciaSemanalModel Miercoles { get; set; } = new();
    public DiaAsistenciaSemanalModel Jueves { get; set; } = new();
    public DiaAsistenciaSemanalModel Viernes { get; set; } = new();
    public DiaAsistenciaSemanalModel Sabado { get; set; } = new();
    public DiaAsistenciaSemanalModel Domingo { get; set; } = new();

    public int Faltas { get; set; }
    public int DiasLaboralesEvaluables { get; set; }
    public int DiasConAsistencia { get; set; }
    public List<DiaAsistenciaSemanalModel> FaltasDetalle { get; set; } = new();
    public List<DiaAsistenciaSemanalModel> DiasPeriodo { get; set; } = new();

    public IEnumerable<DiaAsistenciaSemanalModel> TodosLosDias => DiasPeriodo;

    public string HorarioPeriodoResumen
    {
        get
        {
            var diasHorario = DiasPeriodo
                .Where(d => !string.IsNullOrWhiteSpace(d.HorarioAsignado))
                .OrderBy(d => d.Fecha)
                .ToList();

            if (diasHorario.Count == 0)
                return "Sin horario individual (Jornada laboral general)";

            static string Abrev(DayOfWeek d) => d switch
            {
                DayOfWeek.Monday => "Lun",
                DayOfWeek.Tuesday => "Mar",
                DayOfWeek.Wednesday => "Mié",
                DayOfWeek.Thursday => "Jue",
                DayOfWeek.Friday => "Vie",
                DayOfWeek.Saturday => "Sáb",
                DayOfWeek.Sunday => "Dom",
                _ => ""
            };

            static string RangoDias(List<DiaAsistenciaSemanalModel> grupo)
            {
                var unicos = grupo
                    .Select(x => x.Fecha.DayOfWeek)
                    .Distinct()
                    .OrderBy(d => d == DayOfWeek.Sunday ? 7 : (int)d)
                    .ToList();

                if (unicos.Count == 1)
                    return Abrev(unicos[0]);

                bool consecutivos = true;
                for (int i = 1; i < unicos.Count; i++)
                {
                    int ant = unicos[i - 1] == DayOfWeek.Sunday ? 7 : (int)unicos[i - 1];
                    int act = unicos[i] == DayOfWeek.Sunday ? 7 : (int)unicos[i];
                    if (act != ant + 1)
                    {
                        consecutivos = false;
                        break;
                    }
                }

                return consecutivos
                    ? $"{Abrev(unicos.First())}-{Abrev(unicos.Last())}"
                    : string.Join(",", unicos.Select(Abrev));
            }

            var porHorario = diasHorario
                .GroupBy(d => d.HorarioAsignado.Trim(), StringComparer.CurrentCultureIgnoreCase)
                .Select(h =>
                {
                    var configuraciones = h
                        .GroupBy(d => new
                        {
                            d.Laboral,
                            Entrada = (d.EntradaProgramada ?? "").Trim(),
                            Salida = (d.SalidaProgramada ?? "").Trim()
                        })
                        .Select(g =>
                        {
                            var lista = g.OrderBy(x => x.Fecha).ToList();
                            var dias = RangoDias(lista);

                            if (!g.Key.Laboral)
                                return $"{dias} descanso";

                            var entrada = string.IsNullOrWhiteSpace(g.Key.Entrada) ? "--:--" : g.Key.Entrada;
                            var salida = string.IsNullOrWhiteSpace(g.Key.Salida) ? "--:--" : g.Key.Salida;
                            return $"{dias} {entrada}-{salida}";
                        })
                        .ToList();

                    return $"{h.Key} — {string.Join(" | ", configuraciones)}";
                })
                .ToList();

            return string.Join(" / ", porHorario);
        }
    }
}

public class FiltroAsistenciaSemanalModel
{
    public string Pin { get; set; } = "";
    public string NumeroEmpleado { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Area { get; set; } = "";
    public string Puesto { get; set; } = "";
    public string Faltas { get; set; } = "Todos";
}
