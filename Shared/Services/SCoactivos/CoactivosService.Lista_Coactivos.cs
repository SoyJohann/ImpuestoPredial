using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SCoactivos.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SCoactivos;

public partial class CoactivosService
{
    public Task<Result<List<CoactivoDto>>> Lista_Coactivos(int opcion, string pcomp, int pvigencia, int ptran, int ptipo) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            var query = SelectCoactivos($"reporte_coactivos({opcion}, '{pcomp}', {pvigencia}, {ptran}, {ptipo}) ORDER BY id");
            return (await con.QueryAsync<CoactivoDto>(query)).ToList();
        }, "No se encontraron registros coactivos.");
}
