-- ─────────────────────────────────────────────────────────────────────────────
-- Colaboradores de un mantenimiento (varios tecnicos por servicio).
-- El tecnico RESPONSABLE sigue en mantenimiento.tecnico_id (y el supervisor en
-- supervisor_id); esta tabla guarda a los tecnicos colaboradores adicionales.
--
-- Pendiente de ejecutar en Supabase.
-- ─────────────────────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS public.mantenimiento_tecnico (
  id               serial PRIMARY KEY,
  mantenimiento_id integer NOT NULL REFERENCES public.mantenimiento(id) ON DELETE CASCADE,
  usuario_id       integer NOT NULL REFERENCES public.usuario(id),
  CONSTRAINT uq_mantenimiento_tecnico UNIQUE (mantenimiento_id, usuario_id)
);

CREATE INDEX IF NOT EXISTS idx_mantenimiento_tecnico_mant ON public.mantenimiento_tecnico(mantenimiento_id);
