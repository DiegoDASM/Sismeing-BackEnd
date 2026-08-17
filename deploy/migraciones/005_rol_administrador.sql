-- ============================================================================
-- 005_rol_administrador.sql
--
-- Normaliza la etiqueta del rol 30: estaba guardada como 'ADMINISTRADOR' (en
-- mayusculas). Ya no rompe permisos (el JWT emite el rol canonico por ID y las
-- busquedas de aprobadores tambien son por ID), pero la etiqueta se muestra en
-- pantallas y correos, asi que se deja consistente con el resto de roles.
--
-- SEGURIDAD: solo actualiza el texto del rol 30 si aun esta en mayusculas.
-- ============================================================================

UPDATE public.rol
SET rol = 'Administrador'
WHERE id = 30 AND rol = 'ADMINISTRADOR';
