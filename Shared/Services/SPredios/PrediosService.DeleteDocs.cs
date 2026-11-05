using Coactivos_Predial.Shared.Services.Common;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result> DeleteDocs(int id) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "DELETE FROM \"Documentos\" WHERE \"Id\" = @id";
            await con.ExecuteAsync(query, new { id });
        }, "No fue posible eliminar el documento.");
}
