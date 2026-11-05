using System.Data;
using Coactivos_Predial.Shared.Services.Common;
using Npgsql;
using NpgsqlTypes;

namespace Coactivos_Predial.Shared.Services.Spersuasivo;

public partial class PersuasivoService
{
    public Task<Result> Procesa_Per(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var connection = await connFactory.CreateOpenAsync();
            await using var cmd = new NpgsqlCommand
            {
                CommandText = "SELECT public.persuasivo(@opcion, @pcomp, @pdesde, @phasta, @pvigencia, @ptran)",
                CommandType = CommandType.Text,
                Connection = connection
            };

            cmd.Parameters.Add(new NpgsqlParameter("@opcion", NpgsqlDbType.Integer) { Value = opcion });
            cmd.Parameters.Add(new NpgsqlParameter("@pcomp", NpgsqlDbType.Text) { Value = pcomp });
            cmd.Parameters.Add(new NpgsqlParameter("@pdesde", NpgsqlDbType.Date) { Value = pdesde });
            cmd.Parameters.Add(new NpgsqlParameter("@phasta", NpgsqlDbType.Date) { Value = phasta });
            cmd.Parameters.Add(new NpgsqlParameter("@pvigencia", NpgsqlDbType.Integer) { Value = pvigencia });
            cmd.Parameters.Add(new NpgsqlParameter("@ptran", NpgsqlDbType.Integer) { Value = ptran });

            await cmd.ExecuteNonQueryAsync();
        }, "No fue posible procesar el persuasivo.");
}
