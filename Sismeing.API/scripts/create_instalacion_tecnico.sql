-- ─────────────────────────────────────────────────────────────────────────────
-- Colaboradores de una instalacion (varios tecnicos por servicio).
-- El tecnico RESPONSABLE sigue en instalacion.tecnico_id; esta tabla guarda a
-- los tecnicos colaboradores adicionales. Tabla puente simple (sin auditoria).
--
-- Pendiente de ejecutar en Supabase.
-- ─────────────────────────────────────────────────────────────────────────────

CREATE TABLE IF NOT EXISTS public.instalacion_tecnico (
  id             serial PRIMARY KEY,
  instalacion_id integer NOT NULL REFERENCES public.instalacion(id) ON DELETE CASCADE,
  usuario_id     integer NOT NULL REFERENCES public.usuario(id),
  CONSTRAINT uq_instalacion_tecnico UNIQUE (instalacion_id, usuario_id)
);

CREATE INDEX IF NOT EXISTS idx_instalacion_tecnico_inst ON public.instalacion_tecnico(instalacion_id);
