-- ─────────────────────────────────────────────────────────────────────────────
-- Tipos de trabajo de un contrato (Instalacion y/o Mantenimiento).
-- Un contrato puede cubrir instalacion, mantenimiento o ambos. La Visita Tecnica
-- (id 3) NO se ofrece como tipo de contrato. Tabla puente simple.
--
-- contrato.tipo_trabajo_id se conserva como tipo primario (primero elegido) para
-- no romper lecturas existentes; esta tabla guarda el conjunto completo.
--
-- Pendiente de ejecutar en Supabase.
-- ─────────────────────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS public.contrato_tipo_trabajo (
  id              serial PRIMARY KEY,
  contrato_id     integer NOT NULL REFERENCES public.contrato(id) ON DELETE CASCADE,
  tipo_trabajo_id integer NOT NULL REFERENCES public.tipo_trabajo(id),
  CONSTRAINT uq_contrato_tipo_trabajo UNIQUE (contrato_id, tipo_trabajo_id)
);

CREATE INDEX IF NOT EXISTS idx_contrato_tipo_trabajo_con ON public.contrato_tipo_trabajo(contrato_id);

-- Migracion: sembrar el conjunto desde el tipo_trabajo_id actual de cada contrato.
INSERT INTO public.contrato_tipo_trabajo (contrato_id, tipo_trabajo_id)
SELECT id, tipo_trabajo_id FROM public.contrato
WHERE tipo_trabajo_id IS NOT NULL
ON CONFLICT (contrato_id, tipo_trabajo_id) DO NOTHING;
