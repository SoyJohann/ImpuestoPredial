using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class ConvertirTipoDocumentoAEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos");

            // Remapear al valor del enum TipoDocumento (Predial = 10)
            // antes de eliminar los catálogos.
            migrationBuilder.Sql("""
                UPDATE "Documentos" d
                SET "TipoDocumentoId" = 10
                FROM "TiposDocumento" td
                JOIN "Sectores" s ON s."Id" = td."SectorId"
                WHERE d."TipoDocumentoId" = td."Id"
                  AND s."Codigo" = 'PREDIAL'
                  AND td."Codigo" = 'POR_CLASIFICAR';
                """);

            // Aborta la migración si queda algún documento con un tipo documental
            // sin valor equivalente en el enum TipoDocumento.
            migrationBuilder.Sql("""
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
                """);

            migrationBuilder.DropTable(
                name: "TiposDocumento");

            migrationBuilder.DropTable(
                name: "Sectores");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_TipoDocumentoId",
                table: "Documentos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sectores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SectorId = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposDocumento_Sectores_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectores_Codigo",
                table: "Sectores",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposDocumento_SectorId_Codigo",
                table: "TiposDocumento",
                columns: new[] { "SectorId", "Codigo" },
                unique: true);

            // Regresar solo tiene sentido en desarrollo: resembrar los catálogos
            // con los mismos valores del seed manual y devolver los remapeos
            // ANTES de crear la FK, porque PostgreSQL valida las filas existentes.
            migrationBuilder.Sql("""
                INSERT INTO "Sectores" ("Id", "Codigo", "Nombre", "Activo") VALUES
                    (1, 'PREDIAL', 'Predial', true),
                    (2, 'INDUSTRIA_COMERCIO', 'Industria y Comercio', true);

                INSERT INTO "TiposDocumento" ("Id", "SectorId", "Codigo", "Nombre", "Activo") VALUES
                    (1, 1, 'POR_CLASIFICAR', 'Por clasificar', true),
                    (2, 2, 'POR_CLASIFICAR', 'Por clasificar', true);

                UPDATE "Documentos"
                SET "TipoDocumentoId" = td."Id"
                FROM "TiposDocumento" td
                JOIN "Sectores" s ON s."Id" = td."SectorId"
                WHERE "Documentos"."TipoDocumentoId" = 10
                  AND s."Codigo" = 'PREDIAL'
                  AND td."Codigo" = 'POR_CLASIFICAR';

                SELECT setval(pg_get_serial_sequence('"Sectores"', 'Id'), 2, true);
                SELECT setval(pg_get_serial_sequence('"TiposDocumento"', 'Id'), 2, true);
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId",
                principalTable: "TiposDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
