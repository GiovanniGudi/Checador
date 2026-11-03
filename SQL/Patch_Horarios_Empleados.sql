/*
 PSSF / Checador
 Módulo: Horarios de empleados
 - Sólo crea objetos App* en la BD administrativa.
 - No modifica tablas ZKTeco / checadores.
 - Conserva historial por vigencia.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

IF OBJECT_ID(N'dbo.AppHorarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppHorarios(
        IdHorario            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppHorarios PRIMARY KEY,
        Nombre               NVARCHAR(150) NOT NULL,
        HashConfiguracion    VARCHAR(64) NOT NULL,
        Activo               BIT NOT NULL CONSTRAINT DF_AppHorarios_Activo DEFAULT(1),
        FechaCreacion        DATETIME2(0) NOT NULL CONSTRAINT DF_AppHorarios_FechaCreacion DEFAULT(SYSDATETIME()),
        UsuarioCreacion      NVARCHAR(100) NULL
    );
    CREATE UNIQUE INDEX UX_AppHorarios_HashConfiguracion
        ON dbo.AppHorarios(HashConfiguracion);
END;

IF OBJECT_ID(N'dbo.AppHorarioDetalle', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppHorarioDetalle(
        IdDetalle            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppHorarioDetalle PRIMARY KEY,
        IdHorario            INT NOT NULL,
        DiaSemana            TINYINT NOT NULL, -- 1=Lunes ... 7=Domingo
        EsLaboral            BIT NOT NULL,
        HoraEntrada          TIME(0) NULL,
        HoraSalida           TIME(0) NULL,
        ToleranciaRetardoMin INT NOT NULL CONSTRAINT DF_AppHorarioDetalle_Tolerancia DEFAULT(0),
        CONSTRAINT FK_AppHorarioDetalle_Horario FOREIGN KEY(IdHorario)
            REFERENCES dbo.AppHorarios(IdHorario),
        CONSTRAINT CK_AppHorarioDetalle_Dia CHECK(DiaSemana BETWEEN 1 AND 7),
        CONSTRAINT CK_AppHorarioDetalle_Horas CHECK(
            EsLaboral = 0 OR (HoraEntrada IS NOT NULL AND HoraSalida IS NOT NULL)
        ),
        CONSTRAINT UQ_AppHorarioDetalle UNIQUE(IdHorario, DiaSemana)
    );
END;

IF OBJECT_ID(N'dbo.AppEmpleadoHorario', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppEmpleadoHorario(
        IdAsignacion          BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppEmpleadoHorario PRIMARY KEY,
        NumeroEmpleado        NVARCHAR(50) NOT NULL, -- dbo.Empleados.pin_empleado
        PinChecador           NVARCHAR(50) NULL,     -- dbo.Empleados.id_checador_principal
        NombreEmpleadoSnapshot NVARCHAR(250) NULL,
        UbicacionSnapshot     NVARCHAR(250) NULL,
        IdHorario             INT NOT NULL,
        VigenteDesde          DATE NOT NULL,
        VigenteHasta          DATE NULL,
        FechaCambio           DATETIME2(0) NOT NULL CONSTRAINT DF_AppEmpleadoHorario_FechaCambio DEFAULT(SYSDATETIME()),
        UsuarioCambio         NVARCHAR(100) NULL,
        CONSTRAINT FK_AppEmpleadoHorario_Horario FOREIGN KEY(IdHorario)
            REFERENCES dbo.AppHorarios(IdHorario),
        CONSTRAINT CK_AppEmpleadoHorario_Vigencia CHECK(
            VigenteHasta IS NULL OR VigenteHasta >= VigenteDesde
        )
    );

    CREATE INDEX IX_AppEmpleadoHorario_EmpleadoVigencia
        ON dbo.AppEmpleadoHorario(NumeroEmpleado, VigenteDesde, VigenteHasta);

    CREATE INDEX IX_AppEmpleadoHorario_PinVigencia
        ON dbo.AppEmpleadoHorario(PinChecador, VigenteDesde, VigenteHasta);
END;

IF OBJECT_ID(N'dbo.AppEmpleadoHorarioCambios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppEmpleadoHorarioCambios(
        IdCambio              BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppEmpleadoHorarioCambios PRIMARY KEY,
        NumeroEmpleado        NVARCHAR(50) NOT NULL,
        PinChecador           NVARCHAR(50) NULL,
        IdHorarioAnterior     INT NULL,
        IdHorarioNuevo        INT NOT NULL,
        VigenteDesde          DATE NOT NULL,
        FechaRegistro         DATETIME2(0) NOT NULL CONSTRAINT DF_AppEmpleadoHorarioCambios_Fecha DEFAULT(SYSDATETIME()),
        Usuario               NVARCHAR(100) NULL,
        Observaciones         NVARCHAR(500) NULL
    );

    CREATE INDEX IX_AppEmpleadoHorarioCambios_EmpleadoFecha
        ON dbo.AppEmpleadoHorarioCambios(NumeroEmpleado, FechaRegistro DESC);
END;

CREATE OR ALTER VIEW dbo.vw_AppEmpleadoHorarioVigencias
AS
SELECT
    eh.IdAsignacion,
    eh.NumeroEmpleado,
    eh.PinChecador,
    eh.NombreEmpleadoSnapshot AS NombreEmpleado,
    eh.UbicacionSnapshot AS Ubicacion,
    eh.VigenteDesde,
    eh.VigenteHasta,
    h.IdHorario,
    h.Nombre AS NombreHorario,
    d.DiaSemana,
    d.EsLaboral,
    d.HoraEntrada,
    d.HoraSalida,
    d.ToleranciaRetardoMin
FROM dbo.AppEmpleadoHorario eh
INNER JOIN dbo.AppHorarios h ON h.IdHorario = eh.IdHorario
INNER JOIN dbo.AppHorarioDetalle d ON d.IdHorario = h.IdHorario;
GO
