using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SContribuyentes;

public partial class ContribuyentesService
{
    public async Task<Result<List<ContribuyenteDto>>> GetByNit(string nit)
    {
        if (string.IsNullOrWhiteSpace(nit))
        {
            return Result<List<ContribuyenteDto>>.Fail("El NIT no puede estar vacío.", ResultErrorType.Validation);
        }

        return await ResultExecution.ExecuteAsync(async () =>
        {
            var termino = nit.Trim();

            return await tenantDbContext.Contribuyentes
                .AsNoTracking()
                .Where(contribuyente =>
                    contribuyente.IsActive
                    && contribuyente.Propietario.Documento.Trim() == termino)
                .OrderBy(contribuyente => contribuyente.Id)
                .Select(contribuyente => new ContribuyenteDto(
                    contribuyente.Id,
                    contribuyente.NombreComercial,
                    contribuyente.MatriculaMercantil,
                    contribuyente.ActividadEconomica,
                    contribuyente.Direccion,
                    contribuyente.FechaInicioActividad,
                    contribuyente.Estado,
                    contribuyente.Propietario.Nombre,
                    contribuyente.Propietario.Documento))
                .ToListAsync();
        }, "No fue posible consultar los contribuyentes.");
    }
}
