-- Migración para cambiar timestamp a timestamp with time zone
-- Ejecutar este script en Supabase SQL Editor

ALTER TABLE productos 
ALTER COLUMN fecha_creacion TYPE TIMESTAMP WITH TIME ZONE,
ALTER COLUMN fecha_actualizacion TYPE TIMESTAMP WITH TIME ZONE;
