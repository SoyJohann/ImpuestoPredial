using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result> Add(PropietarioDto infractor) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                INSERT INTO propietarios(cedula, tipo_ident, nombre, direccion, tel, licencia, ciudad, correo, apellidos, categoria, ciudad_lic)
                VALUES(@cedula, @tipo_ident, @nombre, @direccion, @tel, @licencia, @ciudad, @correo, @apellidos, @categoria, @ciudad_lic);
                """;
            await con.ExecuteAsync(query, infractor);
        }, "No fue posible guardar el propietario.");
}
