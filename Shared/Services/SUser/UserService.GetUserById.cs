using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SUser.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SUser;

public partial class UserService
{
    public Task<Result<UsuarioDto?>> GetUserById(int id) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await OpenMasterAsync();
            const string query = "SELECT * FROM usuarios WHERE id = @id";
            return await con.QueryFirstOrDefaultAsync<UsuarioDto>(query, new { id });
        }, "No fue posible consultar el usuario.");
}
