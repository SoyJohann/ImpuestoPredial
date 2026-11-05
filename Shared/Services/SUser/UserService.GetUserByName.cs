using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SUser.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SUser;

public partial class UserService
{
    public Task<Result<List<UsuarioDto>>> GetUserByName(string nombre) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await OpenMasterAsync();
            const string query = "SELECT * FROM \"usuarios\" WHERE nombre = @nombre";
            return (await con.QueryAsync<UsuarioDto>(query, new { nombre })).ToList();
        }, "No fue posible consultar los usuarios por nombre.");
}
