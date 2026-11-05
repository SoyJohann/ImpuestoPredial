-- Formaliza la relación actual de los documentos Predial con maestro.Id.
-- Los 44 documentos sin coincidencia se conservan sin PredioId para revisión.

BEGIN;

DO $migration$
BEGIN
    IF EXISTS (
        SELECT btrim(codigo)
        FROM maestro
        GROUP BY btrim(codigo)
        HAVING count(*) > 1
    ) THEN
        RAISE EXCEPTION 'Hay códigos duplicados en maestro; no se puede asignar PredioId sin ambigüedad';
    END IF;
END
$migration$;

ALTER TABLE "Documentos"
    ADD COLUMN IF NOT EXISTS "PredioId" integer;

UPDATE "Documentos" d
SET "PredioId" = m.id
FROM maestro m
WHERE btrim(d."CodigoPredial") = btrim(m.codigo)
  AND d."PredioId" IS NULL;

DO $migration$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'FK_Documentos_maestro_PredioId'
          AND conrelid = '"Documentos"'::regclass
    ) THEN
        ALTER TABLE "Documentos"
            ADD CONSTRAINT "FK_Documentos_maestro_PredioId"
            FOREIGN KEY ("PredioId") REFERENCES maestro(id);
    END IF;
END
$migration$;

CREATE INDEX IF NOT EXISTS "IX_Documentos_PredioId"
    ON "Documentos" ("PredioId");

-- La pantalla aún envía el código predial, pero la búsqueda de documentos usa la FK.
CREATE OR REPLACE FUNCTION public.muestra_pdfs(
    pcodigo text,
    OUT num integer,
    OUT codigo_predial text,
    OUT nombre text,
    OUT id integer,
    OUT fecha date,
    OUT archivo text
)
RETURNS SETOF record
LANGUAGE sql
AS $function$
    SELECT row_number() OVER (ORDER BY d."Id")::integer,
           d."CodigoPredial",
           d."Nombre",
           d."Id",
           d."Fecha",
           d."Archivo"
    FROM "Documentos" d
    INNER JOIN maestro m ON m.id = d."PredioId"
    WHERE btrim(m.codigo) = btrim(pcodigo)
    ORDER BY d."Id";
$function$;

COMMIT;
