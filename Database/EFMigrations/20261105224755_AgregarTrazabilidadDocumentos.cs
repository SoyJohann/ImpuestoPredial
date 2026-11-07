using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class AgregarTrazabilidadDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            // El usuario 1 es la cuenta de sistema; se usa para los documentos existentes.
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Documentos",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Documentos",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Documentos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Documentos",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Documentos");
        }
    }
}
