-- ─────────────────────────────────────────────────────────────────────────────
-- Vincular los TRABAJOS REALIZADOS a su mantenimiento.
-- Agrega la columna mantenimiento_id (nullable) + FK a la tabla trabajo.
--
-- Ejecutar UNA sola vez en el SQL Editor de Supabase.
-- Es nullable para no romper los registros de catálogo de trabajo ya existentes.
-- ─────────────────────────────────────────────────────────────────────────────

ALTER TABLE public.trabajo
  ADD COLUMN IF NOT EXISTS mantenimiento_id integer;

ALTER TABLE public.trabajo
  ADD CONSTRAINT trabajo_mantenimiento_id_fkey
  FOREIGN KEY (mantenimiento_id) REFERENCES public.mantenimiento(id);

-- La consulta principal será "trabajos de un mantenimiento"
CREATE INDEX IF NOT EXISTS idx_trabajo_mantenimiento
  ON public.trabajo (mantenimiento_id);

-- Verificación
select column_name, data_type, is_nullable
from information_schema.columns
where table_schema = 'public' and table_name = 'trabajo'
order by ordinal_position;
