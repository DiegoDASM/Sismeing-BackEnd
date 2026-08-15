-- ============================================================================
-- 001_informe_cache.sql
--
-- Guarda el HTML ya generado de cada informe para no volver a componerlo en
-- cada visualizacion. Antes, abrir un informe releia fotos, mediciones y
-- trabajos y renderizaba la plantilla entera cada vez.
--
-- La invalidacion NO es por tiempo: se compara fecha_generacion con la
-- fecha_modificacion del servicio de origen. Si el servicio cambio despues,
-- se regenera. Si no, se sirve lo guardado.
--
-- SEGURIDAD DE LA MIGRACION: solo CREA una tabla nueva. No altera, renombra
-- ni borra nada existente, asi que es reversible con un simple DROP TABLE y
-- no puede corromper datos en produccion.
--
-- Aplicar:  se ejecuta tal cual en el SQL Editor de Supabase, o con psql.
-- Revertir: DROP TABLE IF EXISTS public.informe_cache;
-- ============================================================================

CREATE TABLE IF NOT EXISTS public.informe_cache (
    id                SERIAL       PRIMARY KEY,

    -- Variante del informe: 'instalacion-datos', 'instalacion-fotografico',
    -- 'mantenimiento-datos', 'mantenimiento-fotografico', 'visita-datos',
    -- 'visita-fotografico', 'equipo-hojavida'.
    tipo              VARCHAR(40)  NOT NULL,

    -- Id del servicio (o equipo) al que pertenece el informe.
    referencia_id     INTEGER      NOT NULL,

    html              TEXT         NOT NULL,

    fecha_generacion  TIMESTAMP    NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),

    -- Un unico HTML guardado por variante e id: al regenerar se actualiza la
    -- fila existente en lugar de acumular copias viejas.
    CONSTRAINT uq_informe_cache_tipo_referencia UNIQUE (tipo, referencia_id)
);

-- La consulta de lectura siempre filtra por (tipo, referencia_id).
CREATE INDEX IF NOT EXISTS ix_informe_cache_lookup
    ON public.informe_cache (tipo, referencia_id);

COMMENT ON TABLE public.informe_cache IS
    'HTML precalculado de los informes. Se regenera solo cuando el servicio de origen cambia.';
