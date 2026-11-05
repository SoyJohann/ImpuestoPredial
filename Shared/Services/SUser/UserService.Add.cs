using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SUser.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SUser;

public partial class UserService
{
    public Task<Result> Add(UsuarioDto usuarios) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            usuarios.Clave = PasswordHash("123");
            usuarios.UsuarioCreoId = 3;

            await using var con = await OpenMasterAsync();
            const string query = """
                INSERT INTO usuarios(nombre, dir, tel, login, clave, nivel, correo, habilitado, municipio, mun)
                VALUES(@nombre, @dir, @tel, @login, @clave, @nivel, @correo, @habilitado, @municipio, @mun);
                """;
            await con.ExecuteAsync(query, usuarios);
        }, "No fue posible crear el usuario.");
}
