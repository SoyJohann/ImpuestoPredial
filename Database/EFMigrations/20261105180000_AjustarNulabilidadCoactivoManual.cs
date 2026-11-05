using Coactivos_Predial.Shared.AppDbContext;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Coactivos_Predial.Database.EFMigrations;

[DbContext(typeof(TenantDbContext))]
[Migration("20261105180000_AjustarNulabilidadCoactivoManual")]
public sealed class AjustarNulabilidadCoactivoManual : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.Sql("ALTER TABLE \"Parametros\" ALTER COLUMN \"CoactivoManual\" DROP NOT NULL;");

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("UPDATE \"Parametros\" SET \"CoactivoManual\" = 0 WHERE \"CoactivoManual\" IS NULL;");
        migrationBuilder.Sql("ALTER TABLE \"Parametros\" ALTER COLUMN \"CoactivoManual\" SET NOT NULL;");
    }
}
