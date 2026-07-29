-- ─────────────────────────────────────────────────────────────────────────────
-- Origen de la medicion: 'instalacion' o 'mantenimiento'.
-- informe_id apunta a la instalacion O al mantenimiento, que son secuencias
-- distintas: sin este discriminador, una instalacion #N y un mantenimiento #N
-- del mismo equipo compartirian sus mediciones. Con 'origen' quedan separadas.
--
-- Pendiente de ejecutar en Supabase.
-- ─────────────────────────────────────────────────────────────────────────────

ALTER TABLE public.medicion ADD COLUMN IF NOT EXISTS origen varchar(20);

-- Migracion de filas existentes: si el informe_id corresponde a una instalacion
-- se marca 'instalacion'; el resto se asume 'mantenimiento'.
UPDATE public.medicion m
   SET origen = 'instalacion'
 WHERE m.origen IS NULL
   AND EXISTS (SELECT 1 FROM public.instalacion i WHERE i.id = m.informe_id);

UPDATE public.medicion m
   SET origen = 'mantenimiento'
 WHERE m.origen IS NULL;

CREATE INDEX IF NOT EXISTS idx_medicion_informe_origen ON public.medicion (informe_id, origen);
