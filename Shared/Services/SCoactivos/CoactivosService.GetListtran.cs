using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SCoactivos.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SCoactivos;

public partial class CoactivosService
{
    public Task<Result<List<TransaccionReporteDto>>> GetListtran(string ptipo, int pclase) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            string query;

            if (pclase == 1)
            {
                query = "select p.num_proc as transaccion, cast(p.fecha_proc as date) as fecha, count(*) as No " +
                        "from procesos p inner join comparendos c on (c.comparendo = p.comparendo) " +
                        "where p.tipo = '" + ptipo + "' and c.clase = " + pclase +
                        " group by p.num_proc, p.fecha_proc";
            }
            else
            {
                query = "select p.num_proc as transaccion, cast(p.fecha_proc as date) as fecha, count(*) as No " +
                        "from procesos p inner join comparendos c on (c.comparendo = p.comparendo) " +
                        "where p.tipo = '" + ptipo + "' group by p.num_proc, p.fecha_proc";
            }

            return (await con.QueryAsync<TransaccionReporteDto>(query)).ToList();
        }, "No se encontraron transacciones.");
}
