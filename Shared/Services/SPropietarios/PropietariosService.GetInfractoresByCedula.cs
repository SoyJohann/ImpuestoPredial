using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result<List<PropietarioDto>>> GetInfractoresByCedula(string cedula) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = "SELECT * FROM propietarios WHERE documento like @cedula limit 20";
            return (await con.QueryAsync<PropietarioDto>(query, new { cedula = cedula + "%" })).ToList();
        }, "No fue posible consultar los propietarios por cédula.");
}
