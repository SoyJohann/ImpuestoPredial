using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SImportados;

public partial class ImportadosService
{
    public Task<Result> Add(Importados import_obj) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                INSERT INTO importados (codigo, cedula, nombre, dir, area_tet, area_con, avaluo, tipo)
                VALUES (@codigo, @cedula, @nombre, @dir, @area_tet, @area_con, @avaluo, @tipo)
                """;
            await con.ExecuteAsync(query, import_obj);
        }, "No fue posible guardar el registro importado.");
}
