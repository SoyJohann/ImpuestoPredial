using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SCoactivos.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SCoactivos;

public partial class CoactivosService
{
    public Task<Result<List<CoactivoDto>>> GetList1(string x) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            var query = SelectCoactivos("sin_coactivo()");
            return (await con.QueryAsync<CoactivoDto>(query)).ToList();
        }, "No se encontraron coactivos.");
}
