-- ─────────────────────────────────────────────────────────────────────────────
-- MANTENIMIENTO → referencia directa al EQUIPO.
-- Un equipo puede recibir mantenimiento sin haber pasado por una instalación
-- registrada (contratos de solo-mantenimiento con equipos ya instalados).
-- instalacion_id queda nullable (histórico / cuando sí hubo instalación).
--
-- Ya ejecutado en Supabase el 2026-07-11 (no hace falta volver a correrlo).
-- ─────────────────────────────────────────────────────────────────────────────

ALTER TABLE public.mantenimiento
  ADD COLUMN IF NOT EXISTS equipo_id integer;

DO $$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'mantenimiento_equipo_id_fkey') THEN
    ALTER TABLE public.mantenimiento
      ADD CONSTRAINT mantenimiento_equipo_id_fkey
      FOREIGN KEY (equipo_id) REFERENCES public.equipo(id);
  END IF;
END $$;

ALTER TABLE public.mantenimiento
  ALTER COLUMN instalacion_id DROP NOT NULL;

-- Backfill: los mantenimientos existentes toman el equipo de su instalación.
UPDATE public.mantenimiento m
SET equipo_id = i.equipo_id
FROM public.instalacion i
WHERE m.instalacion_id = i.id AND m.equipo_id IS NULL;

CREATE INDEX IF NOT EXISTS idx_mantenimiento_equipo ON public.mantenimiento (equipo_id);
