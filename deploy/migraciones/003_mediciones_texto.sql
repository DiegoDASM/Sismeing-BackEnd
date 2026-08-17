-- ============================================================================
-- 003_mediciones_texto.sql
--
-- Las mediciones pasan de numeric a texto: en campo se registran valores como
-- "220/110" (voltaje bifasico) o "12/24" (temperatura ingreso/salida), que una
-- columna numerica rechazaba. El tecnico necesita esos formatos tal cual.
--
-- SEGURIDAD: ALTER TYPE con USING ::text conserva todos los valores existentes
-- (un 220.5 numerico queda como '220.5'). No se borra ni trunca nada.
--
-- Revertir (solo si TODOS los valores siguen siendo numericos):
--   ALTER TABLE public.medicion ALTER COLUMN voltaje TYPE numeric USING voltaje::numeric;
--   (y asi con las 12 columnas)
-- ============================================================================

ALTER TABLE public.medicion ALTER COLUMN voltaje                       TYPE varchar(60) USING voltaje::text;
ALTER TABLE public.medicion ALTER COLUMN frecuencia                    TYPE varchar(60) USING frecuencia::text;
ALTER TABLE public.medicion ALTER COLUMN amp_evaporador_ventilador_rla TYPE varchar(60) USING amp_evaporador_ventilador_rla::text;
ALTER TABLE public.medicion ALTER COLUMN amp_motor_condensadora_rla    TYPE varchar(60) USING amp_motor_condensadora_rla::text;
ALTER TABLE public.medicion ALTER COLUMN amp_compresor_rla             TYPE varchar(60) USING amp_compresor_rla::text;
ALTER TABLE public.medicion ALTER COLUMN presion_succion_psi           TYPE varchar(60) USING presion_succion_psi::text;
ALTER TABLE public.medicion ALTER COLUMN presion_descarga_psi          TYPE varchar(60) USING presion_descarga_psi::text;
ALTER TABLE public.medicion ALTER COLUMN temp_inicial_final_evap_c     TYPE varchar(60) USING temp_inicial_final_evap_c::text;
ALTER TABLE public.medicion ALTER COLUMN temp_inicial_final_cond_c     TYPE varchar(60) USING temp_inicial_final_cond_c::text;
ALTER TABLE public.medicion ALTER COLUMN temp_ingreso_salida_agua_c    TYPE varchar(60) USING temp_ingreso_salida_agua_c::text;
ALTER TABLE public.medicion ALTER COLUMN temperatura_programada_c      TYPE varchar(60) USING temperatura_programada_c::text;
ALTER TABLE public.medicion ALTER COLUMN humedad_relativa_prog_pct     TYPE varchar(60) USING humedad_relativa_prog_pct::text;

COMMENT ON COLUMN public.medicion.voltaje IS
    'Texto libre: admite formatos de campo como 220/110 ademas de valores simples.';
