# Módulo de horarios de empleados

Rama: `feature/horarios-empleados`

## Implementado

- Vista WPF **Horarios de empleados**.
- Búsqueda por nombre, PIN y número de empleado.
- Selección individual o múltiple.
- Configuración diferente por cada día de la semana.
- Día laboral / descanso.
- Hora de entrada y salida.
- Tolerancia de retardo.
- Fecha de vigencia.
- Historial de asignaciones.
- Reutilización de horarios idénticos mediante hash de configuración.
- Exportación Excel y CSV.
- Tablas administrativas `App*`; no se modifican tablas ZKTeco.
- Servicio `HorarioReporteService` que calcula:
  - retardos;
  - faltas;
  - descansos;
  - salidas anticipadas;
  - diferencia contra la jornada programada;
  - jornadas que cruzan medianoche.

## Regla de compatibilidad obligatoria

La integración con el reporte debe conservar este patrón:

```csharp
var resultadoHorario = await horarioReporteService.CalcularAsync(
    claveEmpleado,
    fecha,
    entradaReal,
    salidaReal);

if (!resultadoHorario.UsaHorarioConfigurado)
{
    // EJECUTAR SIN CAMBIOS LA LÓGICA ACTUAL DEL REPORTE.
}
else
{
    // Aplicar resultadoHorario para incidencias basadas en horario.
}
```

**Nunca debe suponerse un horario cuando `UsaHorarioConfigurado == false`.**

## Historial

`AppEmpleadoHorario` trabaja con:

- `VigenteDesde`
- `VigenteHasta`

Al crear una nueva vigencia se cierra la anterior sin modificar periodos previos.
`AppEmpleadoHorarioCambios` conserva auditoría adicional de los cambios.

## Situación del repositorio

El commit del 3 de noviembre de 2026 registra que la fuente más reciente es
`PSSF_PROYECTO_COMPLETO_TABLAS_MAS_ANCHAS_CORREGIDO.zip`, pero dicho ZIP no está
extraído en GitHub. En la rama principal faltan actualmente los archivos reales de
`Services`, `Models` y `Views` usados por el proyecto.

Por ese motivo **no se modificó `AsistenciaService.cs`**: el archivo no existe en el
repositorio publicado. La clase `HorarioReporteService` queda lista para integrarse
en ese punto cuando se incorpore la fuente vigente.

No fusionar esta rama a `main` hasta incorporar primero la versión completa del
proyecto y compilarla con estos cambios.
