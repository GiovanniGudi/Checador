# Selección inteligente de checadas según horario

Cambio aislado en `Services/AsistenciaSemanalService.cs`.

## Comportamiento

- Si el empleado tiene un horario asignado vigente:
  - la entrada se selecciona como la marcación disponible más cercana a la hora de entrada programada;
  - la salida se toma únicamente entre marcaciones posteriores a la entrada seleccionada;
  - se conserva la lógica histórica de cierre usando la última marcación posterior.
- Si el empleado NO tiene horario asignado:
  - se ejecuta exactamente la lógica anterior (primera entrada / última salida).
- No se modifica ni elimina ningún registro de checada.
- No se modifican tablas, sincronización, configuraciones, reportes, exportaciones ni historial.

## Alcance del checkpoint

El repositorio disponible no contiene el `AsistenciaService` actual que lee las marcaciones crudas. Por seguridad, la mejora se aplicó en el último punto disponible antes de construir el reporte, sin inventar cambios en componentes que no están respaldados.

Los turnos nocturnos siguen reconociéndose en la regla del horario, pero la ampliación de la ventana de lectura al día siguiente debe hacerse en `AsistenciaService` cuando esa versión sea respaldada en el repositorio.
