-- ─────────────────────────────────────────────────────────────────────────────
-- Habilitar Supabase Realtime para las tablas del dashboard
--
-- Ejecutar UNA sola vez en el SQL Editor de Supabase
-- (Dashboard -> SQL Editor -> New query).
--
-- Supabase Realtime lee el WAL de Postgres, por lo que los cambios hechos
-- por la API .NET (EF Core) también generan eventos: basta con agregar las
-- tablas a la publicación "supabase_realtime".
--
-- Equivalente por interfaz: Database -> Publications -> supabase_realtime
-- y activar las tablas.
-- ─────────────────────────────────────────────────────────────────────────────

alter publication supabase_realtime add table
  public.empresa,
  public.equipo,
  public.contrato,
  public.visita_tecnica,
  public.marca,
  public.instalacion,
  public.mantenimiento;

-- Verificación: debe listar las 7 tablas
select schemaname, tablename
from pg_publication_tables
where pubname = 'supabase_realtime';

-- ─────────────────────────────────────────────────────────────────────────────
-- NOTA DE SEGURIDAD (RLS):
-- Estas tablas no tienen RLS, así que cualquiera con la anon key puede
-- suscribirse y ver el payload de los eventos (la fila que cambió).
-- El frontend solo usa el evento como "campana" y re-consulta por la API .NET,
-- pero si se quiere ocultar el payload, habilitar RLS en cada tabla:
--
--   alter table public.empresa enable row level security;
--   (la API .NET no se ve afectada: se conecta con el rol postgres,
--    que ignora RLS)
--
-- Con RLS habilitado y SIN políticas de SELECT, Realtime deja de enviar
-- eventos a la anon key. Para seguir recibiendo la notificación sin exponer
-- datos sensibles, crear una política mínima solo si se decide ese camino.
-- Para la fase actual del proyecto, dejar las tablas sin RLS es aceptable.
-- ─────────────────────────────────────────────────────────────────────────────
