using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using ConstructoraAG_WPF_Listo.Models;

namespace ConstructoraAG_WPF_Listo.Services;

public sealed class HorariosEmpleadoService
{
    private const string EnsureSchemaSql = """
IF OBJECT_ID(N'dbo.AppHorarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppHorarios(
        IdHorario INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppHorarios PRIMARY KEY,
        Nombre NVARCHAR(150) NOT NULL,
        HashConfiguracion VARCHAR(64) NOT NULL,
        Activo BIT NOT NULL CONSTRAINT DF_AppHorarios_Activo DEFAULT(1),
        FechaCreacion DATETIME2(0) NOT NULL CONSTRAINT DF_AppHorarios_FechaCreacion DEFAULT(SYSDATETIME()),
        UsuarioCreacion NVARCHAR(100) NULL
    );
    CREATE UNIQUE INDEX UX_AppHorarios_HashConfiguracion ON dbo.AppHorarios(HashConfiguracion);
END;

IF OBJECT_ID(N'dbo.AppHorarioDetalle', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppHorarioDetalle(
        IdDetalle INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppHorarioDetalle PRIMARY KEY,
        IdHorario INT NOT NULL,
        DiaSemana TINYINT NOT NULL,
        EsLaboral BIT NOT NULL,
        HoraEntrada TIME(0) NULL,
        HoraSalida TIME(0) NULL,
        ToleranciaRetardoMin INT NOT NULL CONSTRAINT DF_AppHorarioDetalle_Tolerancia DEFAULT(0),
        CONSTRAINT FK_AppHorarioDetalle_Horario FOREIGN KEY(IdHorario) REFERENCES dbo.AppHorarios(IdHorario),
        CONSTRAINT CK_AppHorarioDetalle_Dia CHECK(DiaSemana BETWEEN 1 AND 7),
        CONSTRAINT CK_AppHorarioDetalle_Horas CHECK(EsLaboral = 0 OR (HoraEntrada IS NOT NULL AND HoraSalida IS NOT NULL)),
        CONSTRAINT UQ_AppHorarioDetalle UNIQUE(IdHorario, DiaSemana)
    );
END;

IF OBJECT_ID(N'dbo.AppEmpleadoHorario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppEmpleadoHorario(
        IdAsignacion BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppEmpleadoHorario PRIMARY KEY,
        NumeroEmpleado NVARCHAR(50) NOT NULL,
        PinChecador NVARCHAR(50) NULL,
        NombreEmpleadoSnapshot NVARCHAR(250) NULL,
        UbicacionSnapshot NVARCHAR(250) NULL,
        IdHorario INT NOT NULL,
        VigenteDesde DATE NOT NULL,
        VigenteHasta DATE NULL,
        FechaCambio DATETIME2(0) NOT NULL CONSTRAINT DF_AppEmpleadoHorario_FechaCambio DEFAULT(SYSDATETIME()),
        UsuarioCambio NVARCHAR(100) NULL,
        CONSTRAINT FK_AppEmpleadoHorario_Horario FOREIGN KEY(IdHorario) REFERENCES dbo.AppHorarios(IdHorario),
        CONSTRAINT CK_AppEmpleadoHorario_Vigencia CHECK(VigenteHasta IS NULL OR VigenteHasta >= VigenteDesde)
    );
    CREATE INDEX IX_AppEmpleadoHorario_EmpleadoVigencia
        ON dbo.AppEmpleadoHorario(NumeroEmpleado, VigenteDesde, VigenteHasta);
    CREATE INDEX IX_AppEmpleadoHorario_PinVigencia
        ON dbo.AppEmpleadoHorario(PinChecador, VigenteDesde, VigenteHasta);
END;

IF OBJECT_ID(N'dbo.AppEmpleadoHorarioCambios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppEmpleadoHorarioCambios(
        IdCambio BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppEmpleadoHorarioCambios PRIMARY KEY,
        NumeroEmpleado NVARCHAR(50) NOT NULL,
        PinChecador NVARCHAR(50) NULL,
        IdHorarioAnterior INT NULL,
        IdHorarioNuevo INT NOT NULL,
        VigenteDesde DATE NOT NULL,
        FechaRegistro DATETIME2(0) NOT NULL CONSTRAINT DF_AppEmpleadoHorarioCambios_Fecha DEFAULT(SYSDATETIME()),
        Usuario NVARCHAR(100) NULL,
        Observaciones NVARCHAR(500) NULL
    );
    CREATE INDEX IX_AppEmpleadoHorarioCambios_EmpleadoFecha
        ON dbo.AppEmpleadoHorarioCambios(NumeroEmpleado, FechaRegistro DESC);
END;
""";

    public async Task EnsureSchemaAsync()
    {
        using var cn = GetAdminConnection();
        await OpenIfNeededAsync(cn);
        using var cmd = new SqlCommand(EnsureSchemaSql, cn) { CommandTimeout = 60 };
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<EmpleadoHorarioItem>> BuscarEmpleadosAsync(string? texto)
    {
        await EnsureSchemaAsync();
        texto = (texto ?? "").Trim();

        const string sql = """
SELECT TOP (500)
    COALESCE(CONVERT(NVARCHAR(50), e.pin_empleado), N'') AS NumeroEmpleado,
    COALESCE(CONVERT(NVARCHAR(50), e.id_checador_principal), N'') AS Pin,
    COALESCE(e.nombre_completo, N'') AS NombreCompleto,
    COALESCE(e.ubicacion, N'') AS Ubicacion,
    COALESCE(CONVERT(NVARCHAR(50), e.estado), N'') AS Estado,
    COALESCE(h.Nombre, N'SIN HORARIO') AS HorarioActual,
    a.VigenteDesde,
    a.VigenteHasta
FROM dbo.Empleados e
OUTER APPLY (
    SELECT TOP (1) x.*
    FROM dbo.AppEmpleadoHorario x
    WHERE x.NumeroEmpleado = CONVERT(NVARCHAR(50), e.pin_empleado)
      AND CAST(GETDATE() AS DATE) >= x.VigenteDesde
      AND (x.VigenteHasta IS NULL OR CAST(GETDATE() AS DATE) <= x.VigenteHasta)
    ORDER BY x.VigenteDesde DESC, x.IdAsignacion DESC
) a
LEFT JOIN dbo.AppHorarios h ON h.IdHorario = a.IdHorario
WHERE @Texto = N''
   OR COALESCE(e.nombre_completo, N'') LIKE N'%' + @Texto + N'%'
   OR COALESCE(CONVERT(NVARCHAR(50), e.pin_empleado), N'') LIKE N'%' + @Texto + N'%'
   OR COALESCE(CONVERT(NVARCHAR(50), e.id_checador_principal), N'') LIKE N'%' + @Texto + N'%'
ORDER BY e.nombre_completo;
""";

        using var cn = GetAdminConnection();
        await OpenIfNeededAsync(cn);
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Texto", texto);

        var result = new List<EmpleadoHorarioItem>();
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            result.Add(new EmpleadoHorarioItem
            {
                NumeroEmpleado = rd.GetString(0),
                Pin = rd.GetString(1),
                NombreCompleto = rd.GetString(2),
                Ubicacion = rd.GetString(3),
                Estado = rd.GetString(4),
                HorarioActual = rd.GetString(5),
                VigenteDesde = rd.IsDBNull(6) ? null : rd.GetDateTime(6),
                VigenteHasta = rd.IsDBNull(7) ? null : rd.GetDateTime(7)
            });
        }
        return result;
    }

    public async Task AsignarHorarioAsync(
        IReadOnlyCollection<EmpleadoHorarioItem> empleados,
        string nombreHorario,
        IReadOnlyCollection<HorarioDiaEdit> dias,
        DateTime vigenteDesde,
        string? usuario)
    {
        if (empleados.Count == 0)
            throw new InvalidOperationException("Seleccione por lo menos un empleado.");

        ValidateDays(dias);
        await EnsureSchemaAsync();

        using var cn = GetAdminConnection();
        await OpenIfNeededAsync(cn);
        using var tx = (SqlTransaction)await cn.BeginTransactionAsync();

        try
        {
            var idHorario = await GetOrCreateScheduleAsync(
                cn, tx, nombreHorario, dias, usuario);

            foreach (var empleado in empleados)
                await AssignEmployeeAsync(cn, tx, empleado, idHorario, vigenteDesde.Date, usuario);

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<HorarioVigente?> ObtenerHorarioVigenteAsync(string claveEmpleado, DateTime fecha)
    {
        await EnsureSchemaAsync();
        const string sql = """
SELECT TOP (1)
    a.IdAsignacion, a.IdHorario, a.NumeroEmpleado, COALESCE(a.PinChecador,N''),
    h.Nombre, a.VigenteDesde, a.VigenteHasta
FROM dbo.AppEmpleadoHorario a
INNER JOIN dbo.AppHorarios h ON h.IdHorario = a.IdHorario
WHERE (a.NumeroEmpleado = @Clave OR a.PinChecador = @Clave)
  AND @Fecha >= a.VigenteDesde
  AND (a.VigenteHasta IS NULL OR @Fecha <= a.VigenteHasta)
ORDER BY a.VigenteDesde DESC, a.IdAsignacion DESC;
""";

        using var cn = GetAdminConnection();
        await OpenIfNeededAsync(cn);
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Clave", claveEmpleado ?? "");
        cmd.Parameters.AddWithValue("@Fecha", fecha.Date);

        HorarioVigente? horario = null;
        using (var rd = await cmd.ExecuteReaderAsync())
        {
            if (await rd.ReadAsync())
            {
                horario = new HorarioVigente
                {
                    IdAsignacion = rd.GetInt64(0),
                    IdHorario = rd.GetInt32(1),
                    NumeroEmpleado = rd.GetString(2),
                    PinChecador = rd.GetString(3),
                    NombreHorario = rd.GetString(4),
                    VigenteDesde = rd.GetDateTime(5),
                    VigenteHasta = rd.IsDBNull(6) ? null : rd.GetDateTime(6)
                };
            }
        }

        if (horario is null)
            return null;

        horario.Dias = await LoadDaysAsync(cn, horario.IdHorario);
        return horario;
    }

    public async Task<ReglaHorarioDia?> ObtenerReglaDiaAsync(string claveEmpleado, DateTime fecha)
    {
        var horario = await ObtenerHorarioVigenteAsync(claveEmpleado, fecha);
        if (horario is null)
            return null; // Fundamental: SIN HORARIO => el reporte conserva su lógica anterior.

        var dayNumber = ToDayNumber(fecha.DayOfWeek);
        var dia = horario.Dias.FirstOrDefault(x => x.DiaSemana == dayNumber);
        if (dia is null)
            return null;

        return new ReglaHorarioDia
        {
            IdHorario = horario.IdHorario,
            NombreHorario = horario.NombreHorario,
            Fecha = fecha.Date,
            EsLaboral = dia.EsLaboral,
            HoraEntrada = dia.HoraEntradaTime,
            HoraSalida = dia.HoraSalidaTime,
            ToleranciaRetardoMin = dia.ToleranciaRetardoMin
        };
    }

    public async Task<List<HistorialHorarioItem>> ObtenerHistorialAsync(string numeroEmpleado)
    {
        await EnsureSchemaAsync();
        const string sql = """
SELECT a.IdAsignacion, a.NumeroEmpleado, COALESCE(a.PinChecador,N''),
       h.Nombre, a.VigenteDesde, a.VigenteHasta, a.FechaCambio,
       COALESCE(a.UsuarioCambio,N'')
FROM dbo.AppEmpleadoHorario a
INNER JOIN dbo.AppHorarios h ON h.IdHorario = a.IdHorario
WHERE a.NumeroEmpleado = @Numero
ORDER BY a.VigenteDesde DESC, a.IdAsignacion DESC;
""";
        using var cn = GetAdminConnection();
        await OpenIfNeededAsync(cn);
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Numero", numeroEmpleado);

        var result = new List<HistorialHorarioItem>();
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            result.Add(new HistorialHorarioItem
            {
                IdAsignacion = rd.GetInt64(0),
                NumeroEmpleado = rd.GetString(1),
                PinChecador = rd.GetString(2),
                NombreHorario = rd.GetString(3),
                VigenteDesde = rd.GetDateTime(4),
                VigenteHasta = rd.IsDBNull(5) ? null : rd.GetDateTime(5),
                FechaCambio = rd.GetDateTime(6),
                UsuarioCambio = rd.GetString(7)
            });
        }
        return result;
    }

    private static async Task<int> GetOrCreateScheduleAsync(
        SqlConnection cn, SqlTransaction tx, string nombre,
        IReadOnlyCollection<HorarioDiaEdit> dias, string? usuario)
    {
        var hash = ComputeHash(dias);

        using (var find = new SqlCommand(
            "SELECT IdHorario FROM dbo.AppHorarios WHERE HashConfiguracion=@Hash;", cn, tx))
        {
            find.Parameters.AddWithValue("@Hash", hash);
            var existing = await find.ExecuteScalarAsync();
            if (existing is not null && existing != DBNull.Value)
                return Convert.ToInt32(existing);
        }

        int id;
        using (var insert = new SqlCommand("""
INSERT INTO dbo.AppHorarios(Nombre,HashConfiguracion,UsuarioCreacion)
OUTPUT INSERTED.IdHorario
VALUES(@Nombre,@Hash,@Usuario);
""", cn, tx))
        {
            insert.Parameters.AddWithValue("@Nombre",
                string.IsNullOrWhiteSpace(nombre) ? "HORARIO" : nombre.Trim());
            insert.Parameters.AddWithValue("@Hash", hash);
            insert.Parameters.AddWithValue("@Usuario", (object?)usuario ?? DBNull.Value);
            id = Convert.ToInt32(await insert.ExecuteScalarAsync());
        }

        foreach (var d in dias.OrderBy(x => x.DiaSemana))
        {
            using var insertDay = new SqlCommand("""
INSERT INTO dbo.AppHorarioDetalle
(IdHorario,DiaSemana,EsLaboral,HoraEntrada,HoraSalida,ToleranciaRetardoMin)
VALUES(@Id,@Dia,@Laboral,@Entrada,@Salida,@Tol);
""", cn, tx);
            insertDay.Parameters.AddWithValue("@Id", id);
            insertDay.Parameters.AddWithValue("@Dia", d.DiaSemana);
            insertDay.Parameters.AddWithValue("@Laboral", d.EsLaboral);
            insertDay.Parameters.AddWithValue("@Entrada",
                d.EsLaboral ? (object)d.HoraEntradaTime!.Value : DBNull.Value);
            insertDay.Parameters.AddWithValue("@Salida",
                d.EsLaboral ? (object)d.HoraSalidaTime!.Value : DBNull.Value);
            insertDay.Parameters.AddWithValue("@Tol", Math.Max(0, d.ToleranciaRetardoMin));
            await insertDay.ExecuteNonQueryAsync();
        }
        return id;
    }

    private static async Task AssignEmployeeAsync(
        SqlConnection cn, SqlTransaction tx, EmpleadoHorarioItem empleado,
        int idHorario, DateTime vigenteDesde, string? usuario)
    {
        int? oldSchedule = null;
        long? exactAssignment = null;

        using (var exact = new SqlCommand("""
SELECT TOP (1) IdAsignacion, IdHorario
FROM dbo.AppEmpleadoHorario
WHERE NumeroEmpleado=@Numero AND VigenteDesde=@Desde
ORDER BY IdAsignacion DESC;
""", cn, tx))
        {
            exact.Parameters.AddWithValue("@Numero", empleado.NumeroEmpleado);
            exact.Parameters.AddWithValue("@Desde", vigenteDesde);
            using var rd = await exact.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                exactAssignment = rd.GetInt64(0);
                oldSchedule = rd.GetInt32(1);
            }
        }

        if (exactAssignment.HasValue)
        {
            using var update = new SqlCommand("""
UPDATE dbo.AppEmpleadoHorario
SET IdHorario=@IdHorario, PinChecador=@Pin, NombreEmpleadoSnapshot=@Nombre,
    UbicacionSnapshot=@Ubicacion, FechaCambio=SYSDATETIME(), UsuarioCambio=@Usuario
WHERE IdAsignacion=@Id;
""", cn, tx);
            update.Parameters.AddWithValue("@IdHorario", idHorario);
            update.Parameters.AddWithValue("@Pin", EmptyToDbNull(empleado.Pin));
            update.Parameters.AddWithValue("@Nombre", EmptyToDbNull(empleado.NombreCompleto));
            update.Parameters.AddWithValue("@Ubicacion", EmptyToDbNull(empleado.Ubicacion));
            update.Parameters.AddWithValue("@Usuario", (object?)usuario ?? DBNull.Value);
            update.Parameters.AddWithValue("@Id", exactAssignment.Value);
            await update.ExecuteNonQueryAsync();
        }
        else
        {
            DateTime? nextStart = null;
            using (var next = new SqlCommand("""
SELECT TOP (1) VigenteDesde
FROM dbo.AppEmpleadoHorario
WHERE NumeroEmpleado=@Numero AND VigenteDesde>@Desde
ORDER BY VigenteDesde;
""", cn, tx))
            {
                next.Parameters.AddWithValue("@Numero", empleado.NumeroEmpleado);
                next.Parameters.AddWithValue("@Desde", vigenteDesde);
                var value = await next.ExecuteScalarAsync();
                if (value is not null && value != DBNull.Value)
                    nextStart = Convert.ToDateTime(value);
            }

            using (var previous = new SqlCommand("""
SELECT TOP (1) IdAsignacion, IdHorario
FROM dbo.AppEmpleadoHorario
WHERE NumeroEmpleado=@Numero
  AND VigenteDesde<@Desde
  AND (VigenteHasta IS NULL OR VigenteHasta>=@Desde)
ORDER BY VigenteDesde DESC, IdAsignacion DESC;
""", cn, tx))
            {
                previous.Parameters.AddWithValue("@Numero", empleado.NumeroEmpleado);
                previous.Parameters.AddWithValue("@Desde", vigenteDesde);
                using var rd = await previous.ExecuteReaderAsync();
                if (await rd.ReadAsync())
                {
                    var previousId = rd.GetInt64(0);
                    oldSchedule = rd.GetInt32(1);
                    await rd.DisposeAsync();

                    using var close = new SqlCommand("""
UPDATE dbo.AppEmpleadoHorario
SET VigenteHasta=DATEADD(DAY,-1,@Desde)
WHERE IdAsignacion=@Id;
""", cn, tx);
                    close.Parameters.AddWithValue("@Desde", vigenteDesde);
                    close.Parameters.AddWithValue("@Id", previousId);
                    await close.ExecuteNonQueryAsync();
                }
            }

            using var insert = new SqlCommand("""
INSERT INTO dbo.AppEmpleadoHorario
(NumeroEmpleado,PinChecador,NombreEmpleadoSnapshot,UbicacionSnapshot,
 IdHorario,VigenteDesde,VigenteHasta,UsuarioCambio)
VALUES(@Numero,@Pin,@Nombre,@Ubicacion,@IdHorario,@Desde,@Hasta,@Usuario);
""", cn, tx);
            insert.Parameters.AddWithValue("@Numero", empleado.NumeroEmpleado);
            insert.Parameters.AddWithValue("@Pin", EmptyToDbNull(empleado.Pin));
            insert.Parameters.AddWithValue("@Nombre", EmptyToDbNull(empleado.NombreCompleto));
            insert.Parameters.AddWithValue("@Ubicacion", EmptyToDbNull(empleado.Ubicacion));
            insert.Parameters.AddWithValue("@IdHorario", idHorario);
            insert.Parameters.AddWithValue("@Desde", vigenteDesde);
            insert.Parameters.AddWithValue("@Hasta",
                nextStart.HasValue ? (object)nextStart.Value.Date.AddDays(-1) : DBNull.Value);
            insert.Parameters.AddWithValue("@Usuario", (object?)usuario ?? DBNull.Value);
            await insert.ExecuteNonQueryAsync();
        }

        using var audit = new SqlCommand("""
INSERT INTO dbo.AppEmpleadoHorarioCambios
(NumeroEmpleado,PinChecador,IdHorarioAnterior,IdHorarioNuevo,VigenteDesde,Usuario,Observaciones)
VALUES(@Numero,@Pin,@Anterior,@Nuevo,@Desde,@Usuario,@Obs);
""", cn, tx);
        audit.Parameters.AddWithValue("@Numero", empleado.NumeroEmpleado);
        audit.Parameters.AddWithValue("@Pin", EmptyToDbNull(empleado.Pin));
        audit.Parameters.AddWithValue("@Anterior", oldSchedule.HasValue ? oldSchedule.Value : DBNull.Value);
        audit.Parameters.AddWithValue("@Nuevo", idHorario);
        audit.Parameters.AddWithValue("@Desde", vigenteDesde);
        audit.Parameters.AddWithValue("@Usuario", (object?)usuario ?? DBNull.Value);
        audit.Parameters.AddWithValue("@Obs",
            exactAssignment.HasValue ? "Actualización de horario con misma fecha de vigencia." : "Nueva vigencia de horario.");
        await audit.ExecuteNonQueryAsync();
    }

    private static async Task<List<HorarioDiaEdit>> LoadDaysAsync(SqlConnection cn, int idHorario)
    {
        const string sql = """
SELECT DiaSemana,EsLaboral,HoraEntrada,HoraSalida,ToleranciaRetardoMin
FROM dbo.AppHorarioDetalle
WHERE IdHorario=@Id ORDER BY DiaSemana;
""";
        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@Id", idHorario);
        var result = new List<HorarioDiaEdit>();
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            var day = rd.GetByte(0);
            result.Add(new HorarioDiaEdit
            {
                DiaSemana = day,
                Dia = DayName(day),
                EsLaboral = rd.GetBoolean(1),
                HoraEntrada = rd.IsDBNull(2) ? "" : ((TimeSpan)rd.GetValue(2)).ToString(@"hh\:mm"),
                HoraSalida = rd.IsDBNull(3) ? "" : ((TimeSpan)rd.GetValue(3)).ToString(@"hh\:mm"),
                ToleranciaRetardoMin = rd.GetInt32(4)
            });
        }
        return result;
    }

    private static void ValidateDays(IReadOnlyCollection<HorarioDiaEdit> dias)
    {
        if (dias.Count != 7 || dias.Select(x => x.DiaSemana).Distinct().Count() != 7)
            throw new InvalidOperationException("El horario debe contener los siete días de la semana.");

        foreach (var d in dias.Where(x => x.EsLaboral))
        {
            if (!d.HoraEntradaTime.HasValue || !d.HoraSalidaTime.HasValue)
                throw new InvalidOperationException($"Capture una hora válida de entrada y salida para {d.Dia}.");
        }
    }

    private static string ComputeHash(IEnumerable<HorarioDiaEdit> dias)
    {
        var normalized = string.Join("|", dias.OrderBy(x => x.DiaSemana).Select(d =>
            $"{d.DiaSemana}:{d.EsLaboral}:{d.HoraEntradaTime}:{d.HoraSalidaTime}:{Math.Max(0, d.ToleranciaRetardoMin)}"));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(normalized))).ToLowerInvariant();
    }

    private static int ToDayNumber(DayOfWeek day) =>
        day == DayOfWeek.Sunday ? 7 : (int)day;

    private static string DayName(int day) => day switch
    {
        1 => "Lunes", 2 => "Martes", 3 => "Miércoles", 4 => "Jueves",
        5 => "Viernes", 6 => "Sábado", 7 => "Domingo", _ => ""
    };

    private static object EmptyToDbNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();

    private static async Task OpenIfNeededAsync(SqlConnection cn)
    {
        if (cn.State != ConnectionState.Open)
            await cn.OpenAsync();
    }

    private static SqlConnection GetAdminConnection()
    {
        var assemblyName = typeof(HorariosEmpleadoService).Assembly.GetName().Name;
        var type = Type.GetType($"ConstructoraAG_WPF_Listo.Services.DatabaseService, {assemblyName}")
                   ?? throw new InvalidOperationException("No se encontró DatabaseService.");

        var method = type.GetMethod("GetConnection", BindingFlags.Public | BindingFlags.Static)
                     ?? throw new InvalidOperationException("DatabaseService.GetConnection() no está disponible.");

        var value = method.Invoke(null, null);
        if (value is SqlConnection sqlConnection)
            return sqlConnection;
        if (value is string connectionString && !string.IsNullOrWhiteSpace(connectionString))
            return new SqlConnection(connectionString);

        throw new InvalidOperationException(
            "DatabaseService.GetConnection() debe devolver SqlConnection o una cadena de conexión.");
    }
}
