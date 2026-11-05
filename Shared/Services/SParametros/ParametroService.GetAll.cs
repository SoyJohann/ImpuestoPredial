using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SParametros.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SParametros;

public partial class ParametroService
{
    public Task<Result<List<ParametroDto>>> GetAll() =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "SELECT * FROM \"Parametros\"";
            return (await con.QueryAsync<ParametroDto>(query)).ToList();
        }, "No fue posible consultar los parámetros.");
}
