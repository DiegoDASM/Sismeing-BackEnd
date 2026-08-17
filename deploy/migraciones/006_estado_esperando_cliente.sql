-- ============================================================================
-- 006_estado_esperando_cliente.sql
--
-- Doble aprobacion de informes: la aprobacion interna (supervisor o
-- administrador) ya no completa el informe, sino que lo deja en el nuevo
-- estado 'Esperando Cliente'; el cliente de la empresa da la segunda
-- aprobacion y recien ahi pasa a 'Completado'.
--
-- El backend busca 'Esperando Cliente' por nombre EXACTO (sensible a
-- mayusculas), igual que 'Pendiente' y 'Completado'.
--
-- SEGURIDAD: INSERT condicional, no duplica si ya existe.
-- ============================================================================

INSERT INTO public.estado (estado, activo, "UsuarioRegistro", "FechaRegistro")
SELECT 'Esperando Cliente', true, 'SYSTEM', now()
WHERE NOT EXISTS (
    SELECT 1 FROM public.estado e
    WHERE lower(trim(e.estado)) = 'esperando cliente'
);
