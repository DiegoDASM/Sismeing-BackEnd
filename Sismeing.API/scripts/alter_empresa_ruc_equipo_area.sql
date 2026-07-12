-- ─────────────────────────────────────────────────────────────────────────────
-- 1) RUC del cliente (empresa).
-- 2) Área de trabajo dentro del equipo (como aparece en los informes:
--    "ÁREA: PREPARACIÓN"). FK opcional a area_empresa.
--
-- Ya ejecutado en Supabase el 2026-07-02 (no hace falta volver a correrlo).
-- ─────────────────────────────────────────────────────────────────────────────

ALTER TABLE public.empresa
  ADD COLUMN IF NOT EXISTS ruc varchar(13);

ALTER TABLE public.equipo
  ADD COLUMN IF NOT EXISTS area_id integer;

DO $$
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM pg_constraint WHERE conname = 'equipo_area_id_fkey'
  ) THEN
    ALTER TABLE public.equipo
      ADD CONSTRAINT equipo_area_id_fkey
      FOREIGN KEY (area_id) REFERENCES public.area_empresa(id);
  END IF;
END $$;

CREATE INDEX IF NOT EXISTS idx_equipo_area ON public.equipo (area_id);
