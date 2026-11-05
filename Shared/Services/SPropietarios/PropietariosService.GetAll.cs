using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result<List<PropietarioDto>>> GetAll() =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "SELECT * FROM propietarios limit 10";
            return (await con.QueryAsync<PropietarioDto>(query)).ToList();
        }, "No fue posible consultar los propietarios.");
}
