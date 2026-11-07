using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Coactivos_Predial.Database;

/// <summary>Traslada una sola vez los datos de Predial_Cimitarra a Cimitarra.</summary>
/// <remarks>
/// "TipoDocumentoId" ya no referencia a un catálogo: se asigna el valor del enum
/// TipoDocumento (10 Predial, 20 Industria y Comercio). Un tipo sin mapping
/// queda NULL y falla al insertar, para detectar la novedad a tiempo.
/// </remarks>
internal static class LegacyDataMigration
{
    private const string SourceDatabase = "Predial_Cimitarra";
    private const string TargetDatabase = "Cimitarra";
    private const int BatchSize = 500;

    private sealed record Transfer(string Table, string Query);

    private static readonly Transfer[] Transfers =
    [
        new("TiposIdentificacion", "SELECT id AS \"Id\", nombre AS \"Nombre\" FROM tipos_ident UNION ALL SELECT DISTINCT p.tipo_id AS \"Id\", 'Tipo de identificación ' || p.tipo_id::text AS \"Nombre\" FROM propietarios p WHERE p.tipo_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM tipos_ident ti WHERE ti.id = p.tipo_id) ORDER BY 1"),
        new("Propietarios", "SELECT id AS \"Id\", documento AS \"Documento\", tipo_id AS \"TipoId\", nombre AS \"Nombre\", estado_civil AS \"EstadoCivil\", tel AS \"Telefono\", correo AS \"Correo\", fallecido AS \"Fallecido\" FROM propietarios ORDER BY id"),
        new("Maestro", "SELECT m.id AS \"Id\", m.codigo AS \"Codigo\", m.dir AS \"Direccion\", m.matricula AS \"Matricula\", m.area_tet AS \"AreaTerreno\", m.area_con AS \"AreaConstruida\", m.municipio AS \"EsMunicipio\", m.exento AS \"Exento\", m.avaluo AS \"Avaluo\", m.estrato AS \"Estrato\", m.desenglobo AS \"Desenglobo\", m.zona AS \"Zona\", m.barrio AS \"Barrio\", m.ley550 AS \"Ley550\", p.id AS \"PropietarioId\" FROM maestro m JOIN propietarios p ON p.documento = m.cedula AND p.tipo_id = m.tipo ORDER BY m.id"),
        new("PrediosProp", "SELECT pp.id AS \"Id\", m.id AS \"MaestroId\", p.id AS \"PropietarioId\" FROM predios_prop pp JOIN maestro m ON m.codigo = pp.no_predial JOIN propietarios p ON p.documento = pp.cedula AND p.tipo_id = pp.tipo_id LEFT JOIN propietarios principal ON principal.documento = m.cedula AND principal.tipo_id = m.tipo WHERE p.id <> principal.id ORDER BY pp.id"),
        new("Procesos", "SELECT id AS \"Id\", 2 AS \"TipoProceso\", codigo AS \"Codigo\", cedula AS \"Cedula\", fecha AS \"Fecha\", valor AS \"Valor\", CASE WHEN desde IS NULL THEN NULL ELSE make_date(desde, 1, 1) END AS \"Desde\", CASE WHEN hasta IS NULL THEN NULL ELSE make_date(hasta, 1, 1) END AS \"Hasta\", estado AS \"Estado\", pago AS \"Pago\", res_embargo AS \"ResolucionEmbargo\", fecha_embargo AS \"FechaEmbargo\", vigencia AS \"Vigencia\", fecha_mdto AS \"FechaMandamiento\", sem1 AS \"SemestreUno\", sem2 AS \"SemestreDos\", resolucion AS \"Resolucion\", codigo_rap AS \"CodigoRap\", matricula AS \"Matricula\" FROM procesos ORDER BY id"),
        new("Documentos", "SELECT d.\"Id\" AS \"Id\", CASE WHEN s.\"Codigo\" = 'PREDIAL' AND td.\"Codigo\" = 'POR_CLASIFICAR' THEN 10 WHEN s.\"Codigo\" = 'INDUSTRIA_COMERCIO' AND td.\"Codigo\" = 'POR_CLASIFICAR' THEN 20 ELSE NULL END AS \"TipoDocumentoId\", m.id AS \"PredioId\", CASE WHEN m.id IS NULL THEN d.\"CodigoPredial\" ELSE NULL END AS \"CodigoPredial\", d.\"Nombre\" AS \"Nombre\", d.\"Fecha\" AS \"Fecha\", d.\"Ruta\" AS \"Ruta\", d.\"Archivo\" AS \"Archivo\" FROM \"Documentos\" d JOIN \"TiposDocumento\" td ON td.\"Id\" = d.\"TipoDocumentoId\" JOIN \"Sectores\" s ON s.\"Id\" = td.\"SectorId\" LEFT JOIN maestro m ON m.codigo = d.\"CodigoPredial\" ORDER BY d.\"Id\""),
        new("Importados", "SELECT id AS \"Id\", codigo AS \"Codigo\", cedula AS \"Cedula\", nombre AS \"Nombre\", dir AS \"Direccion\", matricula AS \"Matricula\", area_tet AS \"AreaTerreno\", area_con AS \"AreaConstruida\", tipo AS \"Tipo\", municipio AS \"EsMunicipio\", exento AS \"Exento\", avaluo AS \"Avaluo\", estrato AS \"Estrato\", desenglobo AS \"Desenglobo\", zona AS \"Zona\" FROM importados ORDER BY id"),
        new("Parametros", "SELECT id AS \"Id\", nit AS \"Nit\", dir AS \"Direccion\", tel AS \"Telefono\", divipo AS \"Divipo\", ciudad AS \"Ciudad\", nombre AS \"Nombre\", fec_ini_interes AS \"FechaInicioInteres\", vlr_recibo AS \"ValorRecibo\", correo AS \"Correo\", fecha_nueva_ley AS \"FechaNuevaLey\", fin_nueva_ley AS \"FinNuevaLey\", cobra_adicional AS \"CobraAdicional\", coactivo_man AS \"CoactivoManual\", dias_persuasivo AS \"DiasPersuasivo\", dias_coactivo AS \"DiasCoactivo\", dias_mdto AS \"DiasMandamiento\", costas AS \"Costas\", cuenta_recaudo AS \"CuentaRecaudo\" FROM parametros ORDER BY id")
    ];

    public static async Task RunAsync(IConfiguration configuration)
    {
        var template = configuration.GetConnectionString("DefaultConnectionTemplate")
            ?? throw new InvalidOperationException("Falta DefaultConnectionTemplate.");
        var sourceString = template.Replace("{Database}", SourceDatabase, StringComparison.Ordinal);
        var targetString = template.Replace("{Database}", TargetDatabase, StringComparison.Ordinal);

        await using var source = new NpgsqlConnection(sourceString);
        await using var target = new NpgsqlConnection(targetString);
        await source.OpenAsync();
        await target.OpenAsync();

        await using var sourceTransaction = await source.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        await using var targetTransaction = await target.BeginTransactionAsync(IsolationLevel.Serializable);
        await AssertTargetIsEmpty(target, targetTransaction);

        var counts = new Dictionary<string, long>(StringComparer.Ordinal);
        foreach (var transfer in Transfers)
        {
            counts[transfer.Table] = await TransferRows(source, sourceTransaction, target, targetTransaction, transfer);
            Console.WriteLine($"{transfer.Table}: {counts[transfer.Table]}");
        }

        await ResetIdentitySequences(target, targetTransaction);
        await targetTransaction.CommitAsync();
        await sourceTransaction.CommitAsync();
        Console.WriteLine("Migración terminada. Predial_Cimitarra solo se consultó.");
    }

    private static async Task AssertTargetIsEmpty(NpgsqlConnection target, NpgsqlTransaction transaction)
    {
        const string sql = "SELECT count(*) FROM (" +
            "SELECT 1 FROM \"TiposIdentificacion\" UNION ALL SELECT 1 FROM \"Propietarios\" UNION ALL " +
            "SELECT 1 FROM \"Maestro\" UNION ALL SELECT 1 FROM \"PrediosProp\" UNION ALL " +
            "SELECT 1 FROM \"Procesos\" UNION ALL SELECT 1 FROM \"Documentos\" UNION ALL " +
            "SELECT 1 FROM \"Importados\" UNION ALL SELECT 1 FROM \"Parametros\") existing";
        await using var command = new NpgsqlCommand(sql, target, transaction);
        var count = (long)(await command.ExecuteScalarAsync() ?? 0L);
        if (count != 0)
            throw new InvalidOperationException($"Cimitarra ya contiene {count} filas; no se insertó ningún dato.");
    }

    private static async Task<long> TransferRows(NpgsqlConnection source, NpgsqlTransaction sourceTransaction,
        NpgsqlConnection target, NpgsqlTransaction targetTransaction, Transfer transfer)
    {
        await using var select = new NpgsqlCommand(transfer.Query, source, sourceTransaction);
        await using var reader = await select.ExecuteReaderAsync();
        var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
        long count = 0;
        await using var batch = new NpgsqlBatch(target, targetTransaction);

        while (await reader.ReadAsync())
        {
            var command = new NpgsqlBatchCommand(
                $"INSERT INTO \"{transfer.Table}\" ({string.Join(", ", columns.Select(column => $"\"{column}\""))}) " +
                $"VALUES ({string.Join(", ", columns.Select((_, index) => $"@p{index}"))})");
            for (var index = 0; index < columns.Length; index++)
                command.Parameters.AddWithValue($"p{index}", reader.IsDBNull(index) ? DBNull.Value : reader.GetValue(index));
            batch.BatchCommands.Add(command);
            count++;

            if (batch.BatchCommands.Count == BatchSize)
            {
                await batch.ExecuteNonQueryAsync();
                batch.BatchCommands.Clear();
            }
        }

        if (batch.BatchCommands.Count > 0)
            await batch.ExecuteNonQueryAsync();
        return count;
    }

    private static async Task ResetIdentitySequences(NpgsqlConnection target, NpgsqlTransaction transaction)
    {
        foreach (var transfer in Transfers)
        {
            await using var command = new NpgsqlCommand(
                $"SELECT setval(pg_get_serial_sequence('\"{transfer.Table}\"', 'Id'), " +
                $"COALESCE(MAX(\"Id\"), 1), COUNT(*) > 0) FROM \"{transfer.Table}\"", target, transaction);
            await command.ExecuteScalarAsync();
        }
    }
}
