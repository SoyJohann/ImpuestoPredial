using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result<List<ProcesoDto>>> GetCoactivo(string comp) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                SELECT p.id AS "Id", p.codigo AS "Codigo", p.cedula AS "Cedula",
                       p.comparendo AS "Comparendo", p.fecha_mdto AS "FechaMandamiento", p.estado AS "Estado",
                       make_date(p.desde::integer, 1, 1) AS "Desde", make_date(p.hasta::integer, 1, 1) AS "Hasta",
                       e.nombre AS "NombreEstado", p.valor AS "Valor", p.resolucion AS "Resolucion",
                       p.expediente AS "Expediente", p.fecha_proc AS "FechaProceso"
                FROM procesos p
                INNER JOIN estados_proc e ON e.id = p.estado
                WHERE p.comparendo = @comp
                """;
            return (await con.QueryAsync<ProcesoDto>(query, new { comp })).ToList();
        }, "No fue posible consultar los procesos del predio.");
}
