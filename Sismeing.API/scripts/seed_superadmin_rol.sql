-- ─────────────────────────────────────────────────────────────────────────────
-- Rol SuperAdmin (dueño de la plataforma) + asignación a las cuentas dueñas.
--
-- SuperAdmin queda por encima del Administrador: es el único con acceso al
-- panel /admin y control total cross-empresa. El id se fija en 34 para que
-- coincida con la constante ROL.SUPERADMIN del frontend (src/utils/roles.js).
--
-- Roles existentes: 30 Administrador, 31 Tecnico, 32 Supervisor, 33 Cliente.
-- Script idempotente: se puede correr varias veces sin duplicar.
--
-- Pendiente de ejecutar en Supabase.
-- ─────────────────────────────────────────────────────────────────────────────

-- 1) Crear el rol con id fijo 34 (la columna id es serial: acepta id explícito).
INSERT INTO public.rol (id, rol, activo, "FechaRegistro", "UsuarioRegistro")
VALUES (34, 'SuperAdmin', true, now(), 'SEED')
ON CONFLICT (id) DO NOTHING;

-- 2) Mantener la secuencia por delante del id insertado para no chocar en futuros INSERT.
SELECT setval('rol_id_seq', GREATEST(34, (SELECT MAX(id) FROM public.rol)));

-- 3) Asignar SuperAdmin a las dos cuentas dueñas.
--    >>> EDITAR los correos antes de ejecutar <<<
--    (Confirmar en la tabla usuario que existen y están activos.)
UPDATE public.usuario
SET rol_id = 34
WHERE correo_electronico IN (
    'palessandropin2@gmail.com',   -- Paul
    'diegosilvam723@gmail.com'     -- (compañero) — reemplazar si es otro
);

-- 4) Verificación (opcional):
-- SELECT id, correo_electronico, rol_id FROM public.usuario WHERE rol_id = 34;
