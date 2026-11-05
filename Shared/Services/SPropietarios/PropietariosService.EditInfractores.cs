using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result> EditInfractores(PropietarioDto infractor) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                UPDATE infractores
                SET cedula = @cedula, tipo_ident = @tipo_ident, nombre = @nombre,
                    direccion = @direccion, tel = @tel, licencia = @licencia,
                    ciudad = @ciudad, correo = @correo, apellidos = @apellidos,
                    categoria = @categoria, ciudad_lic = @ciudad_lic
                WHERE id = @id
                """;
            await con.ExecuteAsync(query, infractor);
        }, "No fue posible actualizar el propietario.");
}
