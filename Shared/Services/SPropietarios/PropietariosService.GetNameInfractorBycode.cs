using Coactivos_Predial.Shared.Services.Common;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result<string?>> GetNameInfractorBycode(string code, int tipo) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                SELECT apellidos || ' ' || nombre AS nombre
                FROM "infractores"
                WHERE cedula = @codigo AND tipo_ident = @xtipo
                """;
            return await con.QueryFirstOrDefaultAsync<string>(query, new { codigo = code, xtipo = tipo });
        }, "No fue posible consultar el nombre del propietario.");
}
