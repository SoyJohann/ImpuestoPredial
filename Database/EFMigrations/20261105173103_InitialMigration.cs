using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Importados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: false),
                    Matricula = table.Column<string>(type: "text", nullable: false),
                    AreaTerreno = table.Column<int>(type: "integer", nullable: false),
                    AreaConstruida = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    EsMunicipio = table.Column<bool>(type: "boolean", nullable: false),
                    Exento = table.Column<bool>(type: "boolean", nullable: false),
                    Avaluo = table.Column<double>(type: "double precision", nullable: false),
                    Estrato = table.Column<string>(type: "text", nullable: false),
                    Desenglobo = table.Column<bool>(type: "boolean", nullable: false),
                    Zona = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Importados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parametros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nit = table.Column<string>(type: "text", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: false),
                    Divipo = table.Column<string>(type: "text", nullable: false),
                    Ciudad = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    FechaInicioInteres = table.Column<DateTime>(type: "date", nullable: false),
                    ValorRecibo = table.Column<double>(type: "double precision", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    FechaNuevaLey = table.Column<DateTime>(type: "date", nullable: false),
                    FinNuevaLey = table.Column<DateTime>(type: "date", nullable: false),
                    CobraAdicional = table.Column<bool>(type: "boolean", nullable: false),
                    CoactivoManual = table.Column<bool>(type: "boolean", nullable: false),
                    DiasPersuasivo = table.Column<int>(type: "integer", nullable: false),
                    DiasCoactivo = table.Column<int>(type: "integer", nullable: false),
                    DiasMandamiento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Procesos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoProceso = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: true),
                    Comparendo = table.Column<string>(type: "text", nullable: true),
                    Cedula = table.Column<string>(type: "text", nullable: true),
                    Nombres = table.Column<string>(type: "text", nullable: true),
                    Infraccion = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Ciudad = table.Column<string>(type: "text", nullable: true),
                    Departamento = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "date", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "date", nullable: true),
                    FechaSancion = table.Column<DateTime>(type: "date", nullable: true),
                    FechaMandamiento = table.Column<DateTime>(type: "date", nullable: true),
                    FechaProceso = table.Column<DateTime>(type: "date", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: true),
                    NombreEstado = table.Column<string>(type: "text", nullable: true),
                    Desde = table.Column<DateTime>(type: "date", nullable: true),
                    Hasta = table.Column<DateTime>(type: "date", nullable: true),
                    SemestreUno = table.Column<int>(type: "integer", nullable: true),
                    SemestreDos = table.Column<int>(type: "integer", nullable: true),
                    Valor = table.Column<double>(type: "double precision", nullable: false),
                    Resolucion = table.Column<string>(type: "text", nullable: true),
                    Expediente = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procesos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propietarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TipoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    EstadoCivil = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Correo = table.Column<string>(type: "text", nullable: true),
                    Fallecido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propietarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sectores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Maestro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Matricula = table.Column<string>(type: "text", nullable: true),
                    AreaTerreno = table.Column<int>(type: "integer", nullable: false),
                    AreaConstruida = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: true),
                    NombreTipo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    EsMunicipio = table.Column<bool>(type: "boolean", nullable: false),
                    Exento = table.Column<bool>(type: "boolean", nullable: false),
                    Avaluo = table.Column<double>(type: "double precision", nullable: false),
                    TipoId = table.Column<int>(type: "integer", nullable: false),
                    Estrato = table.Column<string>(type: "text", nullable: true),
                    Desenglobo = table.Column<bool>(type: "boolean", nullable: false),
                    Zona = table.Column<string>(type: "text", nullable: true),
                    Barrio = table.Column<int>(type: "integer", nullable: false),
                    Ley550 = table.Column<bool>(type: "boolean", nullable: false),
                    PropietarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maestro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maestro_Propietarios_PropietarioId",
                        column: x => x.PropietarioId,
                        principalTable: "Propietarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SectorId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoDocumentoId = table.Column<int>(type: "integer", nullable: false),
                    PredioId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    Ruta = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Maestro_PredioId",
                        column: x => x.PredioId,
                        principalTable: "Maestro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TiposDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_PredioId",
                table: "Documentos",
                column: "PredioId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Maestro_Codigo",
                table: "Maestro",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maestro_PropietarioId",
                table: "Maestro",
                column: "PropietarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Propietarios_TipoId_Documento",
                table: "Propietarios",
                columns: new[] { "TipoId", "Documento" },
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "Importados");

            migrationBuilder.DropTable(
                name: "Parametros");

            migrationBuilder.DropTable(
                name: "Procesos");

            migrationBuilder.DropTable(
                name: "Maestro");

            migrationBuilder.DropTable(
                name: "TiposDocumento");

            migrationBuilder.DropTable(
                name: "Propietarios");

            migrationBuilder.DropTable(
                name: "Sectores");
        }
    }
}
