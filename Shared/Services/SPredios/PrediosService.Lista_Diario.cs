using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result<List<DocumentoDiarioDto>>> Lista_Diario(DateTime pdesde, DateTime phasta) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                SELECT num AS "Num", codigo_predial AS "CodigoPredial", fecha AS "Fecha"
                FROM pdfs_cargados(@pdesde, @phasta)
                """;
            return (await con.QueryAsync<DocumentoDiarioDto>(query, new { pdesde, phasta })).ToList();
        }, "No fue posible consultar el reporte diario.");
}
