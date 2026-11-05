using System.Data;
using Coactivos_Predial.Shared.Services.Common;
using Npgsql;
using NpgsqlTypes;

namespace Coactivos_Predial.Shared.Services.SImportados;

public partial class ImportadosService
{
    public Task<Result> Procesa_Rec(int puser) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var connection = await connFactory.CreateOpenAsync();
            await using var cmd = new NpgsqlCommand
            {
                CommandText = "SELECT public.procesa_rec(@puser)",
                CommandType = CommandType.Text,
                Connection = connection
            };
            cmd.Parameters.Add(new NpgsqlParameter("@puser", NpgsqlDbType.Integer) { Value = puser });
            await cmd.ExecuteNonQueryAsync();
        }, "No fue posible procesar los registros de recaudo.");
}
