# Snapshot Checador — 22/10/2026

Checkpoint solicitado por el usuario.

Estado funcional a conservar:
- Compatibilidad con N bases de datos de checadores configuradas y activas.
- Sin nombres de BD fijos en la lógica de asistencias.
- Consolidación por PIN y relación con la tabla administrativa Empleados.
- Entrada y salida pueden provenir de bases distintas.
- Motor de jornadas configurable, con cruce de medianoche y sin reutilizar checadas.
- Conservación de conexión, base, ubicación y terminal de origen.
- Asistencias, faltas, Dashboard, Excel, correos e Historial deben usar la misma lógica consolidada.
- Bases de checadores actuales y futuras en modo solo lectura.
- Configuración, historial y auxiliares se guardan únicamente en la BD administrativa.

Tablas:
- ZKTeco empleados: hr_employee
- Administrativa: Empleados

Este archivo marca el punto de respaldo solicitado.
