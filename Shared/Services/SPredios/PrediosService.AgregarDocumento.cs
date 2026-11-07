using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.SPredios.Requests;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.SPredios;

public partial class PrediosService
{
    public Task<Result> AgregarDocumento(AgregarDocumentoRequest request)
    {
        var invalido = Validar(request);
        if (invalido is not null) return Task.FromResult(invalido);

        return ResultExecution.ExecuteAsync(async () =>
        {
            // El módulo viaja en el request: la vista de Industria y Comercio reutiliza
            // este método pasando Modulo.IndustriaComercio.
            var userId = await userContext.GetUserIdAsync();
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");
            }

            var documento = new Documentos
            {
                PredioId = request.PredioId,
                Modulo = request.Modulo,
                Nombre = request.Nombre.Trim(),
                Ruta = request.NombreArchivo.Trim(),
                Fecha = request.Fecha ?? DateTime.Today,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await tenantDbContext.Documentos!.AddAsync(documento);
            await tenantDbContext.SaveChangesAsync();
        }, "No fue posible guardar el documento.");
    }

    private static Result? Validar(AgregarDocumentoRequest r)
    {
        if (r.PredioId <= 0)
            return Result.Fail("Consulte un predio antes de agregarle un documento.", ResultErrorType.Validation);
        if (!Enum.IsDefined(r.Modulo))
            return Result.Fail("Seleccione un módulo válido.", ResultErrorType.Validation);
        if (string.IsNullOrWhiteSpace(r.Nombre))
            return Result.Fail("Ingrese el detalle del documento.", ResultErrorType.Validation);
        if (string.IsNullOrWhiteSpace(r.NombreArchivo)
            || !string.Equals(Path.GetExtension(r.NombreArchivo), ".pdf", StringComparison.OrdinalIgnoreCase))
            return Result.Fail("El documento debe ser un archivo PDF.", ResultErrorType.Validation);

        return null;
    }
}