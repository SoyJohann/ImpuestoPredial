-- Convierte el catálogo de tipos de documento a valores fijos del enum TipoDocumento.
-- El código ya no consulta las tablas: los tipos documentales viven en el enum
-- (10 Predial, 20 Industria y Comercio) y de cada tipo se deriva la carpeta de
-- almacenamiento {Municipio}/{Sector}.
-- Aplicar una vez por base municipal, después de respaldarla.
-- Espejo de la migración EF 20261106155416_ConvertirTipoDocumentoAEnum.

BEGIN;

-- Liberar la FK para poder cambiar los valores fuera del catálogo.
ALTER TABLE "Documentos"
    DROP CONSTRAINT IF EXISTS "FK_Documentos_TiposDocumento_TipoDocumentoId";

UPDATE "Documentos" d
SET "TipoDocumentoId" = 10
FROM "TiposDocumento" td
JOIN "Sectores" s ON s."Id" = td."SectorId"
WHERE d."TipoDocumentoId" = td."Id"
  AND s."Codigo" = 'PREDIAL'
  AND td."Codigo" = 'POR_CLASIFICAR';

-- Aborta la migración si queda algún documento con un tipo documental
-- sin valor equivalente en el enum TipoDocumento.
DO $remap$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM "Documentos" d
        JOIN "TiposDocumento" td ON td."Id" = d."TipoDocumentoId"
        JOIN "Sectores" s ON s."Id" = td."SectorId"
    ) THEN
        RAISE EXCEPTION 'Hay documentos con un tipo documental sin valor en el enum TipoDocumento.';
    END IF;
END
$remap$;

DROP TABLE IF EXISTS "TiposDocumento";
DROP TABLE IF EXISTS "Sectores";
DROP INDEX IF EXISTS "IX_Documentos_TipoDocumentoId";

COMMIT;
