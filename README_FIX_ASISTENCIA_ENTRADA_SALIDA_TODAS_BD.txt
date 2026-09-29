CORRECCIÓN ASISTENCIA - ENTRADA Y SALIDA ENTRE TODAS LAS BASES
==============================================================

Se corrigió AsistenciaService para que la jornada no dependa de la primera base.

Ahora el módulo:
- Consulta las marcaciones individuales de TODAS las conexiones activas.
- Agrupa por PIN + fecha sin importar en qué base se registró la marcación.
- Entrada = primera marcación del día entre todas las BD.
- Salida = última marcación del día entre todas las BD, cuando existen 2 o más registros.
- Terminal = todas las terminales usadas ese día, aunque estén en bases distintas.
- Horas trabajadas = diferencia entre la primera y la última marcación global.

Ejemplo:
BD1: PIN 123, OdataCheck01, 07:02:37
BD2: PIN 123, OdataCheck02, 21:31:51
Resultado: Entrada 07:02:37 / Salida 21:31:51 / Terminal OdataCheck01 / OdataCheck02

No requiere INSERT, UPDATE ni cambios a tablas SQL.
