-- ============================================================================
-- 007_instalacion_observaciones.sql
--
-- La instalacion no tenia campo de observaciones (el mantenimiento tiene
-- observacion_inicial/observaciones_finales y la visita tecnica tiene
-- observaciones). El frontend ya lo esperaba en el modal de detalle, pero la
-- columna nunca existio.
--
-- SEGURIDAD: solo agrega una columna nullable; no toca datos existentes.
-- ============================================================================

ALTER TABLE public.instalacion
    ADD COLUMN IF NOT EXISTS observaciones text;
