using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result<List<PropietarioDto>>> PropietariosBypredial(string codigo) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                SELECT x.documento AS "Documento", p.tipo_id AS "TipoId",
                       x.nombre AS "Nombre", t.nombre AS "TipoNombre"
                FROM predios_prop p
                INNER JOIN propietarios x ON x.documento = p.cedula AND x.tipo_id = p.tipo_id
                INNER JOIN tipos_ident t ON t.id = x.tipo_id
                WHERE p.no_predial = @codigo
                """;
            return (await con.QueryAsync<PropietarioDto>(query, new { codigo })).ToList();
        }, "No fue posible consultar los propietarios del predio.");
}
