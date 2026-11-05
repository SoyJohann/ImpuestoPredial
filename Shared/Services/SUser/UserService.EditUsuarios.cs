using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SUser.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SUser;

public partial class UserService
{
    public Task<Result> EditUsuarios(UsuarioDto usuarios) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await OpenMasterAsync();
            const string query = """
                UPDATE usuarios
                SET nombre = @nombre, dir = @dir, tel = @tel, login = @login,
                    nivel = @nivel, correo = @correo, habilitado = @habilitado,
                    municipio = @municipio, mun = @mun
                WHERE id = @id
                """;
            await con.ExecuteAsync(query, usuarios);
        }, "No fue posible actualizar el usuario.");
}
