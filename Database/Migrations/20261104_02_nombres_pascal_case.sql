-- Renombra las tablas y columnas documentales a PascalCase.
-- Esta migración se ejecuta después de 20261104_01_sectores_tipos_documento.sql.

BEGIN;

ALTER TABLE sectores RENAME TO "Sectores";
ALTER TABLE "Sectores" RENAME COLUMN id TO "Id";
ALTER TABLE "Sectores" RENAME COLUMN codigo TO "Codigo";
ALTER TABLE "Sectores" RENAME COLUMN nombre TO "Nombre";
ALTER TABLE "Sectores" RENAME COLUMN activo TO "Activo";
ALTER TABLE "Sectores" RENAME CONSTRAINT sectores_pkey TO "PK_Sectores";
ALTER TABLE "Sectores" RENAME CONSTRAINT sectores_codigo_key TO "UQ_Sectores_Codigo";

ALTER TABLE tipos_documento RENAME TO "TiposDocumento";
ALTER TABLE "TiposDocumento" RENAME COLUMN id TO "Id";
ALTER TABLE "TiposDocumento" RENAME COLUMN sector_id TO "SectorId";
ALTER TABLE "TiposDocumento" RENAME COLUMN codigo TO "Codigo";
ALTER TABLE "TiposDocumento" RENAME COLUMN nombre TO "Nombre";
ALTER TABLE "TiposDocumento" RENAME COLUMN activo TO "Activo";
ALTER TABLE "TiposDocumento" RENAME CONSTRAINT tipos_documento_pkey TO "PK_TiposDocumento";
ALTER TABLE "TiposDocumento" RENAME CONSTRAINT uq_tipos_documento_sector_codigo TO "UQ_TiposDocumento_Sector_Codigo";
ALTER TABLE "TiposDocumento" RENAME CONSTRAINT tipos_documento_sector_id_fkey TO "FK_TiposDocumento_Sector";

ALTER TABLE documentos RENAME TO "Documentos";
ALTER TABLE "Documentos" RENAME COLUMN id TO "Id";
ALTER TABLE "Documentos" RENAME COLUMN codigo_predial TO "CodigoPredial";
ALTER TABLE "Documentos" RENAME COLUMN nombre TO "Nombre";
ALTER TABLE "Documentos" RENAME COLUMN fecha TO "Fecha";
ALTER TABLE "Documentos" RENAME COLUMN archivo TO "Archivo";
ALTER TABLE "Documentos" RENAME COLUMN tipo_documento_id TO "TipoDocumentoId";
ALTER TABLE "Documentos" RENAME COLUMN ruta TO "Ruta";
ALTER TABLE "Documentos" RENAME CONSTRAINT docspk TO "PK_Documentos";
ALTER TABLE "Documentos" RENAME CONSTRAINT fk_documentos_tipo_documento TO "FK_Documentos_TipoDocumento";

ALTER SEQUENCE documentos_id_seq RENAME TO "Documentos_Id_seq";
ALTER SEQUENCE sectores_id_seq RENAME TO "Sectores_Id_seq";
ALTER SEQUENCE tipos_documento_id_seq RENAME TO "TiposDocumento_Id_seq";

-- Conserva las columnas de salida históricas que consumen las pantallas Dapper.
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
    WHERE d."CodigoPredial" = pcodigo
    ORDER BY d."Id";
$function$;

COMMIT;
