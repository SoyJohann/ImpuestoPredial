using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result<ResumenPdfsDto>> Pdfs_Resumen(DateTime pdesde, DateTime phasta) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                SELECT count(*) AS item, sum(num) AS num
                FROM pdfs_cargados(@pdesde, @phasta)
                """;
            return await con.QueryFirstOrDefaultAsync<ResumenPdfsDto>(query, new { pdesde, phasta })
                ?? new ResumenPdfsDto(0, 0);
        }, "No fue posible calcular el resumen de documentos.");
}
