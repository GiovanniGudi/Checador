# Checkpoint — horarios centralizados y relación SQL

Fecha: 2026-11-04

Este checkpoint reemplaza como referencia de trabajo al ZIP anterior descartado.

## Estado funcional que se conserva
- Interfaz horizontal original.
- Módulo Administrador de horarios.
- Horarios con vigencias históricas.
- Asignación de personal a horarios.
- Reporte de asistencia por periodo mostrando el horario del empleado.
- Resumen del horario acorde a la configuración guardada.
- Modo fuera de línea y protección contra operaciones duplicadas.

## Regla de PIN confirmada
Para relacionar empleados se usa el PIN efectivo:

```sql
COALESCE(
    NULLIF(LTRIM(RTRIM(CONVERT(NVARCHAR(50), id_checador_principal))), ''),
    NULLIF(LTRIM(RTRIM(CONVERT(NVARCHAR(50), pin_empleado))), '')
)
```

Si `id_checador_principal` está vacío o NULL, se usa `pin_empleado`.

Ejemplo confirmado:
- pin_empleado = 562
- id_checador_principal = NULL
- PinEfectivo = 562

## Estado actual de HORARIO GENERAL
Se confirmó que existen asignaciones vigentes como:
- 5070 — ABELINO ROMUALDO SANTIAGO
- 562 — FRANCISCO ANTONIO PEREZ CRUZ

Ambos con `VigenteHasta = NULL`.

## Próximo trabajo
A partir de este checkpoint, el trabajo pendiente principal es SQL:
relacionar/asignar masivamente empleados a `HORARIO GENERAL` según sus checadas,
sin duplicar asignaciones y respetando vigencias.

No tomar el ZIP anterior descartado como base.
