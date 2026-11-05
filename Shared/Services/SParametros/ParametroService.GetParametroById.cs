using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SParametros.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SParametros;

public partial class ParametroService
{
    public Task<Result<ParametroDto?>> GetParametroById(int id) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "SELECT * FROM parametros WHERE id = 1";
            return await con.QueryFirstOrDefaultAsync<ParametroDto>(query);
        }, "No fue posible consultar los parámetros.");
}
