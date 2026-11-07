using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.SDatabase;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SEstadisticas;

public sealed class EstadisticasService(ITenantConnectionFactory connFactory) : IEstadisticasService
{
    public Task<Result<ResumenDocumentosDto>> GetResumenDocumentosAsync() =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                SELECT count(*)::int AS Predial
                FROM "Documentos"
                WHERE "Modulo" = @predial
                  AND "IsActive" = true
                """;
            var predial = await con.QueryFirstAsync<int>(query, new { predial = Modulo.Predial });
            return new ResumenDocumentosDto(predial);
        }, "No fue posible contar los documentos de predial.");
}
