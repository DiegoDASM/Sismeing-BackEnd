-- ─────────────────────────────────────────────────────────────────────────────
-- Habilitar Supabase Realtime para las tablas de la aplicación
--
-- Ejecutar en el SQL Editor de Supabase (Dashboard -> SQL Editor -> New query).
-- Es idempotente: se puede volver a ejecutar cada vez que se agregue una tabla
-- a la lista de abajo.
--
-- Supabase Realtime lee el WAL de Postgres, por lo que los cambios hechos
-- por la API .NET (EF Core) también generan eventos: basta con agregar las
-- tablas a la publicación "supabase_realtime".
--
-- Equivalente por interfaz: Database -> Publications -> supabase_realtime
-- y activar las tablas.
-- ─────────────────────────────────────────────────────────────────────────────

-- Se agregan una por una: "add table" falla completo si alguna ya está en la
-- publicación, y este script se re-ejecuta cada vez que aparece una tabla nueva.
do $$
declare
  t text;
  tablas text[] := array[
    -- Operaciones
    'empresa', 'usuario', 'contrato', 'contrato_tipo_trabajo', 'equipo',
    'direccion_empresa', 'area_empresa',
    'instalacion', 'instalacion_tecnico',
    'mantenimiento', 'mantenimiento_tecnico',
    'visita_tecnica', 'medicion', 'notificacion',
    -- Fotos
    'foto_instalacion', 'foto_mantenimiento', 'foto_visita_tecnica',
    -- Catálogos
    'marca', 'modelo', 'tipo_equipo', 'tipo_mantenimiento', 'tipo_trabajo',
    'estado', 'rol', 'trabajo'
  ];
begin
  foreach t in array tablas loop
    if not exists (
      select 1 from pg_publication_tables
      where pubname = 'supabase_realtime' and schemaname = 'public' and tablename = t
    ) then
      execute format('alter publication supabase_realtime add table public.%I', t);
    end if;
  end loop;
end $$;

-- Verificación: debe listar las 25 tablas
select schemaname, tablename
from pg_publication_tables
where pubname = 'supabase_realtime'
order by tablename;

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
