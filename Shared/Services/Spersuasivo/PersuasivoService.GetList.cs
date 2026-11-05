using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.Spersuasivo.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.Spersuasivo;

public partial class PersuasivoService
{
    public Task<Result<List<PersuasivoDto>>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            var query = "SELECT id AS \"Id\", comparendo AS \"Comparendo\", cedula AS \"Cedula\", " +
                        "fecha AS \"Fecha\", nombres AS \"Nombres\", infraccion AS \"Infraccion\", " +
                        "dir AS \"Direccion\", tel AS \"Telefono\", valor AS \"Valor\", ciudad AS \"Ciudad\", " +
                        "fecven AS \"FechaVencimiento\", dpto AS \"Departamento\" FROM sin_persuasivo('" +
                        opcion + "', '" + pcomp + "', '" + pdesde + "', '" + phasta + "', '" + pvigencia + "', '" + ptran + "')";
            return (await con.QueryAsync<PersuasivoDto>(query)).ToList();
        }, "No se encontraron persuasivos.");
}
