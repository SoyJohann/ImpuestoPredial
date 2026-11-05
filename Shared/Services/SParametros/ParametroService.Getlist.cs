using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SParametros.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SParametros;

public partial class ParametroService
{
    public Task<Result<List<EstadoProcesoDto>>> Getlist() =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "SELECT * FROM \"estados_proc\" limit 10";
            return (await con.QueryAsync<EstadoProcesoDto>(query)).ToList();
        }, "No fue posible consultar los estados del proceso.");
}
