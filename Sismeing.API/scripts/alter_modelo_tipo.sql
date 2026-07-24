-- ─────────────────────────────────────────────────────────────────────────────
-- Tipo de equipo del modelo.
-- Permite filtrar los modelos por el tipo de equipo seleccionado (ej. al elegir
-- "Cassette" solo se ofrecen los modelos de ese tipo). FK opcional a tipo_equipo
-- para no romper los modelos existentes, que quedan sin tipo hasta asignárselo.
--
-- Pendiente de ejecutar en Supabase.
-- ─────────────────────────────────────────────────────────────────────────────

ALTER TABLE public.modelo
  ADD COLUMN IF NOT EXISTS tipo_id integer;

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'modelo_tipo_id_fkey'
  ) THEN
    ALTER TABLE public.modelo
      ADD CONSTRAINT modelo_tipo_id_fkey
      FOREIGN KEY (tipo_id) REFERENCES public.tipo_equipo(id);
  END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_modelo_tipo ON public.modelo (tipo_id);
