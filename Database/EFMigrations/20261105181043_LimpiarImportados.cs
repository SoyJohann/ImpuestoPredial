using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class LimpiarImportados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ley550",
                table: "Importados");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ley550",
                table: "Importados",
                type: "boolean",
                nullable: true);
        }
    }
}
