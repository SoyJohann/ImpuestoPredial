using Coactivos_Predial.Shared.Services.Common;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SUser;

public partial class UserService
{
    public Task<Result> Change_key(int pid, string skey) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            var pclave = PasswordHash(skey);
            await using var con = await OpenMasterAsync();
            const string query = "UPDATE usuarios SET clave = @pclave WHERE id = @pid";
            await con.ExecuteAsync(query, new { pclave, pid });
        }, "No fue posible cambiar la clave del usuario.");
}
