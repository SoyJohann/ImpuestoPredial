using Coactivos_Predial.Shared.Services.Common;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SImportados;

public partial class ImportadosService
{
    public Task<Result> DeleteRec() =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            await con.ExecuteAsync("delete from simit_rec;");
        }, "No fue posible limpiar los registros de recaudo.");
}
