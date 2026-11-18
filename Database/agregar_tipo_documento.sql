-- Actualiza una base existente (por ejemplo la del servidor) con la columna
-- TipoDocumento y su índice. Es idempotente: se puede ejecutar más de una vez.
--
-- Equivale a la migración 20261118025159_AgregarTipoDocumento. La clasificación
-- inicial usa el nombre del documento; los que no coinciden quedan en 0
-- (Por clasificar) y se pueden reubicar desde la vista de carpetas.

ALTER TABLE "Documentos"
    ADD COLUMN IF NOT EXISTS "TipoDocumento" integer NOT NULL DEFAULT 0;

CREATE INDEX IF NOT EXISTS "IX_Documentos_Modulo_TipoDocumento_IsActive"
    ON "Documentos" ("Modulo", "TipoDocumento", "IsActive");

UPDATE "Documentos"
SET "TipoDocumento" = CASE
    WHEN "Modulo" = 10 AND "Nombre" ILIKE '%notificaci%' THEN 30
    WHEN "Modulo" = 10 AND "Nombre" ILIKE '%embargo%' THEN 40
    WHEN "Modulo" = 10 AND "Nombre" ILIKE '%resoluci%' THEN 50
    WHEN "Modulo" = 10 AND ("Nombre" ILIKE '%mandamiento%' OR "Nombre" ILIKE 'mand%pago%') THEN 20
    WHEN "Modulo" = 10 AND "Nombre" ILIKE '%liquidaci%' THEN 10
    WHEN "Modulo" = 20 AND "Nombre" ILIKE 'rut%' THEN 60
    WHEN "Modulo" = 20 AND "Nombre" ILIKE '%cámara de comercio%' THEN 70
    WHEN "Modulo" = 20 AND "Nombre" ILIKE '%camara de comercio%' THEN 70
    WHEN "Modulo" = 20 AND "Nombre" ILIKE '%registro mercantil%' THEN 80
    ELSE 0
END
WHERE "TipoDocumento" = 0;
