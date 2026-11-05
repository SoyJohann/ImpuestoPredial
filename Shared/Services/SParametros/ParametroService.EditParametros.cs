using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SParametros.Responses;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SParametros;

public partial class ParametroService
{
    public Task<Result> EditParametros(int id, ParametroDto parametros) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            await using var con = await connFactory.CreateOpenAsync();
            const string query = """
                UPDATE parametros
                SET dir = @dir, ciudad = @ciudad, nit = @nit, tel = @tel,
                    vlr_recibo = @vlr_recibo, cuotas_finan = @cuotas_finan,
                    porc_min_fin = @porc_min_fin, fin_nueva_ley = @fin_nueva_ley,
                    cobra_adicional = @cobra_adicional, nombre = @nombre,
                    tipo_acuerdo = @tipo_acuerdo, tipo_inicial = @tipo_inicial,
                    divipo = @divipo, fec_ini_interes = @fec_ini_interes,
                    dias_persuasivo = @dias_persuasivo, dh_fecha6 = @dh_fecha6
                WHERE id = @id
                """;
            await con.ExecuteAsync(query, parametros);
        }, "No fue posible actualizar los parámetros.");
}
