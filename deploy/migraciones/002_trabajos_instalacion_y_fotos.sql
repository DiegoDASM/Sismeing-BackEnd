-- ============================================================================
-- 002_trabajos_instalacion_y_fotos.sql
--
-- 1) Las instalaciones pasan a tener trabajos realizados, igual que ya los
--    tienen los mantenimientos. Se reutiliza la tabla "trabajo" anadiendole
--    instalacion_id, en paralelo al mantenimiento_id que ya existe.
--
-- 2) Cada foto puede indicar a que trabajo corresponde. Es lo que permite que
--    la subida se presente en recuadros separados por trabajo y que el informe
--    fotografico salga agrupado igual, sin que el tecnico se confunda.
--
-- SEGURIDAD: solo anade columnas ANULABLES. Las filas existentes quedan con
-- NULL y siguen funcionando como hasta ahora (fotos sin trabajo asignado se
-- muestran en su grupo general). No altera ni borra nada.
--
-- Revertir:
--   ALTER TABLE public.trabajo            DROP COLUMN IF EXISTS instalacion_id;
--   ALTER TABLE public.foto_instalacion   DROP COLUMN IF EXISTS trabajo_id;
--   ALTER TABLE public.foto_mantenimiento DROP COLUMN IF EXISTS trabajo_id;
--   ALTER TABLE public.foto_visita_tecnica DROP COLUMN IF EXISTS trabajo_id;
-- ============================================================================

-- 1) Trabajos realizados en instalaciones
ALTER TABLE public.trabajo
    ADD COLUMN IF NOT EXISTS instalacion_id INTEGER NULL
    REFERENCES public.instalacion(id) ON DELETE CASCADE;

CREATE INDEX IF NOT EXISTS ix_trabajo_instalacion
    ON public.trabajo (instalacion_id);

-- 2) Foto -> trabajo al que pertenece.
--    ON DELETE SET NULL: si se quita un trabajo, la foto NO se borra; queda
--    sin agrupar. Perder una evidencia fotografica por reordenar trabajos
--    seria mucho peor que verla en el grupo general.
ALTER TABLE public.foto_instalacion
    ADD COLUMN IF NOT EXISTS trabajo_id INTEGER NULL
    REFERENCES public.trabajo(id) ON DELETE SET NULL;

ALTER TABLE public.foto_mantenimiento
    ADD COLUMN IF NOT EXISTS trabajo_id INTEGER NULL
    REFERENCES public.trabajo(id) ON DELETE SET NULL;

CREATE INDEX IF NOT EXISTS ix_foto_instalacion_trabajo
    ON public.foto_instalacion (trabajo_id);
CREATE INDEX IF NOT EXISTS ix_foto_mantenimiento_trabajo
    ON public.foto_mantenimiento (trabajo_id);

COMMENT ON COLUMN public.trabajo.instalacion_id IS
    'Instalacion a la que pertenece el trabajo. Excluyente con mantenimiento_id; ambos NULL = plantilla de catalogo.';
COMMENT ON COLUMN public.foto_instalacion.trabajo_id IS
    'Trabajo al que corresponde la foto. NULL = foto general del servicio.';
