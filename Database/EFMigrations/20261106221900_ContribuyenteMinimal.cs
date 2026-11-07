using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class ContribuyenteMinimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Agregar tabla Contribuyente (sin tocar Maestro/Predio)
            migrationBuilder.CreateTable(
                name: "Contribuyentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropietarioId = table.Column<int>(type: "integer", nullable: false),
                    NombreComercial = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    MatriculaMercantil = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ActividadEconomica = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FechaInicioActividad = table.Column<DateTime>(type: "date", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contribuyentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contribuyentes_Propietarios_PropietarioId",
                        column: x => x.PropietarioId,
                        principalTable: "Propietarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // 2. Agregar ContribuyenteId a Documentos (nullable, sin FK inmediata)
            migrationBuilder.AddColumn<int>(
                name: "ContribuyenteId",
                table: "Documentos",
                type: "integer",
                nullable: true);

            // 3. Crear índice sobre (Modulo, IsActive) - Modulo ya existe en la DB
            migrationBuilder.CreateIndex(
                name: "IX_Documentos_Modulo_IsActive",
                table: "Documentos",
                columns: new[] { "Modulo", "IsActive" });

            // 4. Actualizar FK de Documentos a ContribuyenteId cuando haya datos
            migrationBuilder.Sql(@"
                COMMENT ON COLUMN Documentos.ContribuyenteId IS 'FK a Contribuyente, NULL = documento de módulo Predial (Modulo=10)'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documentos_Modulo_IsActive",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ContribuyenteId",
                table: "Documentos");

            migrationBuilder.DropTable(
                name: "Contribuyentes");
        }
    }
}
