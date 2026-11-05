using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result<PredioDto?>> GetPrediosByCode(string codigoBusqueda) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                SELECT c.*, x.nombre AS ntipo
                FROM maestro c
                LEFT JOIN tipos_ident x ON x.id = c.tipo
                WHERE c.codigo = @comp
                """;
            return await con.QueryFirstOrDefaultAsync<PredioDto>(query, new { comp = codigoBusqueda });
        }, "No fue posible consultar el predio.");
}
