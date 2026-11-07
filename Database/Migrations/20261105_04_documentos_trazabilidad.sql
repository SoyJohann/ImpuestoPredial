-- Agrega trazabilidad a los documentos.
-- El usuario 1 es la cuenta de sistema y queda asignado a los documentos existentes.
-- Es idempotente: se puede ejecutar en cada base municipal sin repetir columnas.

BEGIN;

ALTER TABLE "Documentos"
    ADD COLUMN IF NOT EXISTS "CreatedBy" integer NOT NULL DEFAULT 1,
    ADD COLUMN IF NOT EXISTS "CreatedAt" timestamp with time zone NOT NULL DEFAULT now(),
    ADD COLUMN IF NOT EXISTS "UpdatedBy" integer NOT NULL DEFAULT 1,
    ADD COLUMN IF NOT EXISTS "UpdatedAt" timestamp with time zone,
    ADD COLUMN IF NOT EXISTS "IsActive" boolean NOT NULL DEFAULT true;

-- Los documentos existentes conservan su fecha documental como fecha de creación.
-- El usuario 1 es la cuenta de sistema.
UPDATE "Documentos"
SET "CreatedAt" = "Fecha"::timestamp AT TIME ZONE 'America/Bogota'
WHERE "CreatedBy" = 1
  AND "Fecha" IS NOT NULL;

COMMIT;
