using Coactivos_Predial.Shared.Services.Common;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SUser;

public partial class UserService
{
    public Task<Result> DeleteUsuarioById(int id) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await OpenMasterAsync();
            const string query = "DELETE FROM usuarios WHERE id = @id";
            await con.ExecuteAsync(query, new { id });
        }, "No fue posible eliminar el usuario.");
}
