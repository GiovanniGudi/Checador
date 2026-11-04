# Checkpoint Horarios — 2026-11-03

Estado guardado a solicitud del usuario.

## Interfaz
- Se conserva la interfaz horizontal original de Constructora AG.
- En **NÓMINA Y JORNADA** existe el acceso **Horarios empleados**.
- No se aplica todavía el ajuste posterior propuesto para autocompletar horas de Sábado.

## Módulo de horarios
Estado funcional visible:
- búsqueda de empleados,
- selección de empleado,
- horario por día,
- días laborables,
- entrada y salida,
- tolerancia de retardo,
- vigencia,
- historial,
- exportación Excel/CSV.

## Comportamiento actual conocido
Si un día está marcado como **Laboral** y no tiene hora de entrada y salida, el sistema muestra:
`Capture una hora válida de entrada y salida para <día>.`

Este comportamiento corresponde al estado mostrado por el usuario y se conserva en este checkpoint.

## Copia completa usada como referencia
Archivo local de checkpoint:
`CHECKPOINT_CHECADOR_HORARIOS_2026-11-03.zip`

SHA-256:
`117beaddf4d37471700a59dd9ef47938c9defb06b251a88e94b9b25535edddab`

La rama de checkpoint parte de `feature/horarios-empleados` y conserva el estado hasta este punto antes de continuar con nuevas correcciones.
