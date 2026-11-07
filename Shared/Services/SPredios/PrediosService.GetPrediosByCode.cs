using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result<PredioDto?>> GetPrediosByCode(string codigoBusqueda) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            var codigo = codigoBusqueda.Trim();

            return await tenantDbContext.Predio
                .AsNoTracking()
                .Where(p => p.Codigo == codigo)
                .Select(p => new PredioDto(
                    p.Id,
                    p.Codigo,
                    p.Direccion ?? string.Empty,
                    p.Matricula,
                    p.AreaTerreno,
                    p.AreaConstruida,
                    p.EsMunicipio,
                    p.Exento,
                    p.Avaluo,
                    p.Estrato,
                    p.Desenglobo,
                    p.Zona,
                    p.Barrio,
                    p.Ley550,
                    p.Propietario != null ? p.Propietario.Nombre : null,
                    p.Propietario != null ? p.Propietario.Documento : null))
                .FirstOrDefaultAsync();
        }, "No fue posible consultar el predio.");
}