using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Coactivos_Predial.Database.EFMigrations
{
    /// <inheritdoc />
    public partial class CompatibilidadDatosHistoricos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_Maestro_PredioId",
                table: "Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Maestro");

            migrationBuilder.DropColumn(
                name: "NombreTipo",
                table: "Maestro");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Maestro");

            migrationBuilder.DropColumn(
                name: "TipoId",
                table: "Maestro");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Importados");

            migrationBuilder.AlterColumn<bool>(
                name: "Fallecido",
                table: "Propietarios",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<double>(
                name: "Valor",
                table: "Procesos",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Procesos",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Procesos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoRap",
                table: "Procesos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEmbargo",
                table: "Procesos",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Matricula",
                table: "Procesos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Pago",
                table: "Procesos",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ResolucionEmbargo",
                table: "Procesos",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Vigencia",
                table: "Procesos",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ValorRecibo",
                table: "Parametros",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Parametros",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinNuevaLey",
                table: "Parametros",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaNuevaLey",
                table: "Parametros",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Divipo",
                table: "Parametros",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "DiasPersuasivo",
                table: "Parametros",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<short>(
                name: "DiasMandamiento",
                table: "Parametros",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "DiasCoactivo",
                table: "Parametros",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Correo",
                table: "Parametros",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "CobraAdicional",
                table: "Parametros",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.Sql("ALTER TABLE \"Parametros\" ALTER COLUMN \"CoactivoManual\" TYPE smallint USING CASE WHEN \"CoactivoManual\" IS NULL THEN NULL WHEN \"CoactivoManual\" THEN 1 ELSE 0 END;");

            migrationBuilder.AddColumn<short>(
                name: "Costas",
                table: "Parametros",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CuentaRecaudo",
                table: "Parametros",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Ley550",
                table: "Maestro",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "Exento",
                table: "Maestro",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "EsMunicipio",
                table: "Maestro",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "Maestro",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Desenglobo",
                table: "Maestro",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<int>(
                name: "Barrio",
                table: "Maestro",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Avaluo",
                table: "Maestro",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "AreaTerreno",
                table: "Maestro",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AreaConstruida",
                table: "Maestro",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Zona",
                table: "Importados",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Importados",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Matricula",
                table: "Importados",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "Exento",
                table: "Importados",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Estrato",
                table: "Importados",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "EsMunicipio",
                table: "Importados",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "Desenglobo",
                table: "Importados",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Importados",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "Avaluo",
                table: "Importados",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "AreaTerreno",
                table: "Importados",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AreaConstruida",
                table: "Importados",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "Ley550",
                table: "Importados",
                type: "boolean",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ruta",
                table: "Documentos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "PredioId",
                table: "Documentos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Documentos",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "Archivo",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoPredial",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrediosProp",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaestroId = table.Column<int>(type: "integer", nullable: false),
                    PropietarioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrediosProp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrediosProp_Maestro_MaestroId",
                        column: x => x.MaestroId,
                        principalTable: "Maestro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrediosProp_Propietarios_PropietarioId",
                        column: x => x.PropietarioId,
                        principalTable: "Propietarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposIdentificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposIdentificacion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrediosProp_MaestroId",
                table: "PrediosProp",
                column: "MaestroId");

            migrationBuilder.CreateIndex(
                name: "IX_PrediosProp_PropietarioId",
                table: "PrediosProp",
                column: "PropietarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_Maestro_PredioId",
                table: "Documentos",
                column: "PredioId",
                principalTable: "Maestro",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId",
                principalTable: "TiposDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Propietarios_TiposIdentificacion_TipoId",
                table: "Propietarios",
                column: "TipoId",
                principalTable: "TiposIdentificacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_Maestro_PredioId",
                table: "Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Propietarios_TiposIdentificacion_TipoId",
                table: "Propietarios");

            migrationBuilder.DropTable(
                name: "PrediosProp");

            migrationBuilder.DropTable(
                name: "TiposIdentificacion");

            migrationBuilder.DropColumn(
                name: "CodigoRap",
                table: "Procesos");

            migrationBuilder.DropColumn(
                name: "FechaEmbargo",
                table: "Procesos");

            migrationBuilder.DropColumn(
                name: "Matricula",
                table: "Procesos");

            migrationBuilder.DropColumn(
                name: "Pago",
                table: "Procesos");

            migrationBuilder.DropColumn(
                name: "ResolucionEmbargo",
                table: "Procesos");

            migrationBuilder.DropColumn(
                name: "Vigencia",
                table: "Procesos");

            migrationBuilder.DropColumn(
                name: "Costas",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "CuentaRecaudo",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "Ley550",
                table: "Importados");

            migrationBuilder.DropColumn(
                name: "Archivo",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "CodigoPredial",
                table: "Documentos");

            migrationBuilder.AlterColumn<bool>(
                name: "Fallecido",
                table: "Propietarios",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Valor",
                table: "Procesos",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Estado",
                table: "Procesos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Procesos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "ValorRecibo",
                table: "Parametros",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Parametros",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinNuevaLey",
                table: "Parametros",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaNuevaLey",
                table: "Parametros",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Divipo",
                table: "Parametros",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiasPersuasivo",
                table: "Parametros",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiasMandamiento",
                table: "Parametros",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiasCoactivo",
                table: "Parametros",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Correo",
                table: "Parametros",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "CobraAdicional",
                table: "Parametros",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.Sql("ALTER TABLE \"Parametros\" ALTER COLUMN \"CoactivoManual\" TYPE boolean USING CASE WHEN \"CoactivoManual\" IS NULL THEN FALSE ELSE \"CoactivoManual\" <> 0 END;");

            migrationBuilder.AlterColumn<bool>(
                name: "Ley550",
                table: "Maestro",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Exento",
                table: "Maestro",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EsMunicipio",
                table: "Maestro",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "Maestro",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "Desenglobo",
                table: "Maestro",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Barrio",
                table: "Maestro",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Avaluo",
                table: "Maestro",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaTerreno",
                table: "Maestro",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaConstruida",
                table: "Maestro",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Maestro",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NombreTipo",
                table: "Maestro",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "Maestro",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoId",
                table: "Maestro",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Zona",
                table: "Importados",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Importados",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Matricula",
                table: "Importados",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Exento",
                table: "Importados",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estrato",
                table: "Importados",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EsMunicipio",
                table: "Importados",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Desenglobo",
                table: "Importados",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Importados",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Avaluo",
                table: "Importados",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaTerreno",
                table: "Importados",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AreaConstruida",
                table: "Importados",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Importados",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Ruta",
                table: "Documentos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PredioId",
                table: "Documentos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Documentos",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_Maestro_PredioId",
                table: "Documentos",
                column: "PredioId",
                principalTable: "Maestro",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                table: "Documentos",
                column: "TipoDocumentoId",
                principalTable: "TiposDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
