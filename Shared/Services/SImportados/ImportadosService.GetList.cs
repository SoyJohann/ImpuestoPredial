using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SImportados.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SImportados;

public partial class ImportadosService
{
    public Task<Result<List<ImportadoDto>>> GetList() =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "SELECT * FROM \"importados\" limit 20";
            return (await con.QueryAsync<ImportadoDto>(query)).ToList();
        }, "No se encontraron registros importados.");
}
