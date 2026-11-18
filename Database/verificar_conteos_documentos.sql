-- Verificación de la migración TiposDocumentoYAlmacenamiento.
-- Ejecutar en cada base municipal (tenant) antes y después de aplicar la migración.
-- Los conteos deben coincidir; ningún documento debe quedar huérfano.

-- ANTES: distribución por el enum anterior (columna TipoDocumento).
-- Guarde este resultado para compararlo con el de DESPUÉS.
SELECT "Modulo", "TipoDocumento", count(*) AS documentos
FROM "Documentos"
GROUP BY "Modulo", "TipoDocumento"
ORDER BY "Modulo", "TipoDocumento";

-- DESPUÉS: distribución por el catálogo TiposDocumento.
SELECT d."Modulo", td."Nombre" AS "TipoDocumento", count(*) AS documentos
FROM "Documentos" d
JOIN "TiposDocumento" td ON td."Id" = d."TipoDocumentoId"
GROUP BY d."Modulo", td."Nombre"
ORDER BY d."Modulo", td."Nombre";

-- DESPUÉS: total, nulos y huérfanos (deben ser 0).
SELECT count(*) AS total,
       count(*) FILTER (WHERE d."TipoDocumentoId" IS NULL) AS sin_tipo,
       count(*) FILTER (WHERE td."Id" IS NULL) AS huerfanos
FROM "Documentos" d
LEFT JOIN "TiposDocumento" td ON td."Id" = d."TipoDocumentoId";

-- DESPUÉS: catálogo sembrado por módulo.
SELECT "Modulo", "Nombre", "Orden", "Activo", "IsActive"
FROM "TiposDocumento"
ORDER BY "Modulo", "Orden", "Nombre";
