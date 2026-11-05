using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SParametros.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SParametros;

public partial class ParametroService
{
    public Task<Result> EditEstados(EstadoProcesoDto estado) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "UPDATE estados_proc SET nombre = @nombre, porc = @porc WHERE id = @id";
            await con.ExecuteAsync(query, estado);
        }, "No fue posible actualizar el estado del proceso.");
}
