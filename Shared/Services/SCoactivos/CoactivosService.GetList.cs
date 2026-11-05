using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SCoactivos.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SCoactivos;

public partial class CoactivosService
{
    public Task<Result<List<CoactivoDto>>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            var query = SelectCoactivos($"sin_Coactivo('{opcion}', '{pcomp}', '{pdesde}', '{phasta}', '{pvigencia}', '{ptran}')");
            return (await con.QueryAsync<CoactivoDto>(query)).ToList();
        }, "No se encontraron coactivos.");
}
