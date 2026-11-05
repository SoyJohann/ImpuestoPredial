using Coactivos_Predial.Shared.Services.Common;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SUser;

public partial class UserService
{
    public Task<Result> Reset_key(int pid) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            var pclave = PasswordHash("123");
            await using var con = await OpenMasterAsync();
            const string query = "UPDATE usuarios SET clave = @pclave WHERE id = @pid";
            await con.ExecuteAsync(query, new { pclave, pid });
        }, "No fue posible restablecer la clave del usuario.");
}
