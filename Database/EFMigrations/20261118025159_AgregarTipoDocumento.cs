using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class AgregarTipoDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La cadena inicial no creaba Modulo; se agrega aquí para que una base
            // vacía pueda recorrer todas las migraciones. En bases existentes es idempotente.
            migrationBuilder.Sql("""
                ALTER TABLE "Documentos" ADD COLUMN IF NOT EXISTS "Modulo" integer NOT NULL DEFAULT 10;
                ALTER TABLE "Documentos" ALTER COLUMN "Modulo" DROP DEFAULT;
                """);

            migrationBuilder.AddColumn<int>(
                name: "TipoDocumento",
                table: "Documentos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // La conversión anterior dejó la columna heredada; en una base vacía
            // impide que la nueva migración renombre la columna. Es idempotente.
            migrationBuilder.Sql("""ALTER TABLE "Documentos" DROP COLUMN IF EXISTS "TipoDocumentoId";""");

            // Clasificación inicial de los documentos existentes según su nombre.
            // Las notificaciones se evalúan antes que el documento base para no
            // confundir "Notificación Liquidación" con una liquidación.
            migrationBuilder.Sql("""
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
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_Modulo_TipoDocumento_IsActive",
                table: "Documentos",
                columns: new[] { "Modulo", "TipoDocumento", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documentos_Modulo_TipoDocumento_IsActive",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "TipoDocumento",
                table: "Documentos");

            // Restaura la columna heredada para que el Down de la migración
            // anterior siga siendo aplicable en una base creada desde cero.
            migrationBuilder.Sql("""
                ALTER TABLE "Documentos" ADD COLUMN IF NOT EXISTS "TipoDocumentoId" integer NOT NULL DEFAULT 10;
                """);

            migrationBuilder.Sql("""ALTER TABLE "Documentos" DROP COLUMN IF EXISTS "Modulo";""");
        }
    }
}
