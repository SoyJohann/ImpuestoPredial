using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result<List<PredioBusquedaDto>>> BuscarPrediosPorCedula(string cedula) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                SELECT codigo AS "CodigoPredial", avaluo AS "Avaluo",
                       dir AS "Direccion", matricula AS "Matricula",
                       s_nulo(nombre) AS "Nombre"
                FROM maestro
                WHERE cedula LIKE @cedula
                ORDER BY codigo
                """;
            return (await con.QueryAsync<PredioBusquedaDto>(query, new { cedula = cedula.Trim() + "%" })).ToList();
        }, "No fue posible consultar los predios por cédula.");
}
