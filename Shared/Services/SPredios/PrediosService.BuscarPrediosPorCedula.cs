using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPredios.Responses;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public async Task<Result<List<PredioBusquedaDto>>> BuscarPrediosPorCedula(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
        {
            return Result<List<PredioBusquedaDto>>.Fail("La cédula no puede estar vacía.", ResultErrorType.Validation);
        }

        return await ResultExecution.ExecuteAsync(async () =>
        {
            var termino = cedula.Trim();

            var predios = await tenantDbContext.Predio
                .AsNoTracking()
                .Where(m => m.Propietario != null && m.Propietario.Documento.StartsWith(termino))
                .OrderBy(m => m.Codigo)
                .Select(m => new PredioBusquedaDto(
                    m.Codigo,
                    m.Avaluo,
                    m.Direccion,
                    m.Matricula,
                    m.Propietario.Nombre != null ? m.Propietario.Nombre.Trim() : string.Empty
                ))
                .ToListAsync();

            return predios;
        }, "No fue posible consultar los predios por cédula.");
    }
}