-- ============================================================================
-- 004_seed_estados.sql
--
-- 1) Siembra el catalogo "estado", que estaba VACIO en produccion: al registrar
--    un mantenimiento o instalacion el backend busca el estado 'Pendiente'
--    (y al aprobar, 'Completado') por nombre EXACTO, y sin filas el registro
--    fallaba con "No existe el estado 'Pendiente' en el catálogo".
--
-- 2) De paso sanea tipo_mantenimiento: 'CORRECTIVO' se guardo con un salto de
--    linea al final (rompia la comparacion por nombre) y faltaba PREDICTIVO,
--    que la interfaz ya contempla.
--
-- SEGURIDAD: solo INSERT condicionales (no duplica si ya existen, sin importar
-- mayusculas) y un TRIM de espacios/saltos. No borra ni pisa nada.
-- ============================================================================

-- 1) Estados del flujo de servicios.
--    'Pendiente' y 'Completado' DEBEN quedar con esta capitalizacion exacta:
--    el backend los busca con comparacion sensible a mayusculas.
INSERT INTO public.estado (estado, activo, "UsuarioRegistro", "FechaRegistro")
SELECT v.nombre, true, 'SYSTEM', now()
FROM (VALUES
    ('Pendiente'),
    ('En Proceso'),
    ('Completado'),
    ('Esperando Repuestos'),
    ('Cancelado'),
    ('Urgente')
) AS v(nombre)
WHERE NOT EXISTS (
    SELECT 1 FROM public.estado e
    WHERE lower(trim(e.estado)) = lower(v.nombre)
);

-- 2) Limpieza de tipo_mantenimiento (saltos de linea y espacios colados).
UPDATE public.tipo_mantenimiento
SET tipo_mantenimiento = trim(both E' \t\r\n' FROM tipo_mantenimiento)
WHERE tipo_mantenimiento <> trim(both E' \t\r\n' FROM tipo_mantenimiento);

-- PREDICTIVO: tercer tipo que la interfaz ya reconoce.
INSERT INTO public.tipo_mantenimiento (tipo_mantenimiento, activo, "UsuarioRegistro", "FechaRegistro")
SELECT 'PREDICTIVO', true, 'SYSTEM', now()
WHERE NOT EXISTS (
    SELECT 1 FROM public.tipo_mantenimiento t
    WHERE lower(trim(t.tipo_mantenimiento)) = 'predictivo'
);
