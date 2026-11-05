using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result<PropietarioDto?>> GetInfractorById(int id) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "SELECT * FROM propietarios WHERE id = @id";
            return await con.QueryFirstOrDefaultAsync<PropietarioDto>(query, new { id });
        }, "No fue posible consultar el propietario.");
}
