-- ─────────────────────────────────────────────────────────────────────────────
-- VISITA TÉCNICA → cliente como texto plano (prospecto).
-- La visita técnica sirve para evaluar si se dará el servicio, por lo que la
-- empresa aún NO es cliente registrado: se guarda su información como texto.
-- empresa_id queda nullable por compatibilidad (visitas históricas).
--
-- Ya ejecutado en Supabase el 2026-07-02 (no hace falta volver a correrlo).
-- ─────────────────────────────────────────────────────────────────────────────

ALTER TABLE public.visita_tecnica
  ALTER COLUMN empresa_id DROP NOT NULL;

ALTER TABLE public.visita_tecnica
  ADD COLUMN IF NOT EXISTS nombre_empresa varchar(200),
  ADD COLUMN IF NOT EXISTS contacto        varchar(150),
  ADD COLUMN IF NOT EXISTS telefono        varchar(20),
  ADD COLUMN IF NOT EXISTS direccion       varchar(300);
