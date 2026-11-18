using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class TiposDocumentoYAlmacenamiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TipoDocumento",
                table: "Documentos",
                newName: "TipoDocumentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Documentos_Modulo_TipoDocumento_IsActive",
                table: "Documentos",
                newName: "IX_Documentos_Modulo_TipoDocumentoId_IsActive");

            migrationBuilder.AddColumn<DateTime>(
                name: "EliminadoEn",
                table: "Documentos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EliminadoPor",
                table: "Documentos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashSha256",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivo",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Documentos",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Modulo = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Id);
                });

            // Semilla idempotente por (Modulo, Nombre): no depende de IDs fijos.
            migrationBuilder.Sql("""
                INSERT INTO "TiposDocumento" ("Modulo", "Nombre", "Orden", "Activo", "CreatedAt", "CreatedBy", "UpdatedBy", "IsActive")
                SELECT v."Modulo", v."Nombre", v."Orden", true, now(), 1, 1, true
                FROM (VALUES
                    (10, 'Liquidación', 10),
                    (10, 'Mandamiento de pago', 20),
                    (10, 'Notificación', 30),
                    (10, 'Embargo', 40),
                    (10, 'Resolución', 50),
                    (10, 'Por clasificar', 90),
                    (20, 'RUT', 10),
                    (20, 'Cámara de comercio', 20),
                    (20, 'Registro mercantil', 30),
                    (20, 'Liquidación', 40),
                    (20, 'Mandamiento de pago', 50),
                    (20, 'Por clasificar', 90)
                ) AS v("Modulo", "Nombre", "Orden")
                WHERE NOT EXISTS (
                    SELECT 1 FROM "TiposDocumento" td
                    WHERE td."Modulo" = v."Modulo" AND td."Nombre" = v."Nombre"
                );
                """);

            // Convierte los valores del enum anterior a la llave de la nueva tabla.
            migrationBuilder.Sql("""
                UPDATE "Documentos" d
                SET "TipoDocumentoId" = td."Id"
                FROM "TiposDocumento" td
                WHERE td."Modulo" = d."Modulo"
                  AND td."Nombre" = CASE d."TipoDocumentoId"
                      WHEN 10 THEN 'Liquidación'
                      WHEN 20 THEN 'Mandamiento de pago'
                      WHEN 30 THEN 'Notificación'
                      WHEN 40 THEN 'Embargo'
                      WHEN 50 THEN 'Resolución'
                      WHEN 60 THEN 'RUT'
                      WHEN 70 THEN 'Cámara de comercio'
                      WHEN 80 THEN 'Registro mercantil'
                      ELSE 'Por clasificar'
                  END;
                """);

            // Cualquier valor sin equivalente queda en "Por clasificar" del módulo.
            migrationBuilder.Sql("""
                UPDATE "Documentos" d
                SET "TipoDocumentoId" = td."Id"
                FROM "TiposDocumento" td
                WHERE td."Modulo" = d."Modulo"
                  AND td."Nombre" = 'Por clasificar'
                  AND NOT EXISTS (SELECT 1 FROM "TiposDocumento" actual WHERE actual."Id" = d."TipoDocumentoId");
                """);

            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM "Documentos" d
                        LEFT JOIN "TiposDocumento" td ON td."Id" = d."TipoDocumentoId"
                        WHERE td."Id" IS NULL
                    ) THEN
                        RAISE EXCEPTION 'Hay documentos sin un tipo documental válido.';
                    END IF;
                END $$;
                """);

            // La migración anterior dejó DEFAULT 0; el tipo se asigna siempre en código.
            migrationBuilder.Sql("""ALTER TABLE "Documentos" ALTER COLUMN "TipoDocumentoId" DROP DEFAULT;""");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposDocumento_Modulo_Nombre",
                table: "TiposDocumento",
                columns: new[] { "Modulo", "Nombre" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId",
                principalTable: "TiposDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos");

            // Restaura los valores del enum anterior antes de eliminar el catálogo.
            migrationBuilder.Sql("""
                UPDATE "Documentos" d
                SET "TipoDocumentoId" = CASE td."Nombre"
                    WHEN 'Liquidación' THEN 10
                    WHEN 'Mandamiento de pago' THEN 20
                    WHEN 'Notificación' THEN 30
                    WHEN 'Embargo' THEN 40
                    WHEN 'Resolución' THEN 50
                    WHEN 'RUT' THEN 60
                    WHEN 'Cámara de comercio' THEN 70
                    WHEN 'Registro mercantil' THEN 80
                    ELSE 0
                END
                FROM "TiposDocumento" td
                WHERE td."Id" = d."TipoDocumentoId";
                """);

            migrationBuilder.DropTable(
                name: "TiposDocumento");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_TipoDocumentoId",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "EliminadoEn",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "EliminadoPor",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "HashSha256",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "NombreArchivo",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Documentos");

            migrationBuilder.Sql("""ALTER TABLE "Documentos" ALTER COLUMN "TipoDocumentoId" SET DEFAULT 0;""");

            migrationBuilder.RenameColumn(
                name: "TipoDocumentoId",
                table: "Documentos",
                newName: "TipoDocumento");

            migrationBuilder.RenameIndex(
                name: "IX_Documentos_Modulo_TipoDocumentoId_IsActive",
                table: "Documentos",
                newName: "IX_Documentos_Modulo_TipoDocumento_IsActive");
        }
    }
}
