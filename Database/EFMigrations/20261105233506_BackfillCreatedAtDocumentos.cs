using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class BackfillCreatedAtDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Los documentos existentes conservan su fecha documental como fecha de creación.
            // El usuario 1 es la cuenta de sistema asignada en la migración anterior.
            migrationBuilder.Sql("""
                UPDATE "Documentos"
                SET "CreatedAt" = "Fecha"::timestamp AT TIME ZONE 'America/Bogota'
                WHERE "CreatedBy" = 1
                  AND "Fecha" IS NOT NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No se revierte: la fecha original de creación no se puede reconstruir.
        }
    }
}
