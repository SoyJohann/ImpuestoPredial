using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.Document;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.Document.Reponses;
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
            // este método pasando Modulo.IndustriaComercio y ContribuyenteId.
            var userId = await userContext.GetUserIdAsync();
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");
            }

            var tipo = await tenantDbContext.TiposDocumento!
                .AsNoTracking()
                .FirstOrDefaultAsync(tipo =>
                    tipo.Id == request.TipoDocumentoId
                    && tipo.Modulo == request.Modulo
                    && tipo.Activo
                    && tipo.IsActive);

            if (tipo is null)
            {
                throw new ArgumentException("Seleccione un tipo de documento válido para el módulo.");
            }

            DocumentoGuardado? guardado = null;
            try
            {
                // Primero se escribe el archivo; si la fila falla, se limpia el huérfano.
                guardado = await storage.GuardarAsync(request.Modulo, request.Contenido!, CancellationToken.None);

                var documento = new Documentos
                {
                    PredioId = request.Modulo == Modulo.Predial ? request.PredioId : null,
                    ContribuyenteId = request.Modulo == Modulo.IndustriaComercio ? request.ContribuyenteId : null,
                    Modulo = request.Modulo,
                    TipoDocumentoId = tipo.Id,
                    Nombre = request.Nombre.Trim(),
                    NombreArchivo = DocumentoStoragePaths.SanitizeFileName(request.NombreArchivo),
                    StorageKey = guardado.StorageKey,
                    MimeType = guardado.MimeType,
                    Size = guardado.Size,
                    HashSha256 = guardado.HashSha256,
                    Fecha = request.Fecha ?? DateTime.Today,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await tenantDbContext.Documentos!.AddAsync(documento);
                await tenantDbContext.SaveChangesAsync();
            }
            catch
            {
                if (guardado is not null)
                {
                    storage.EliminarArchivo(guardado.RutaCompleta);
                }

                throw;
            }
        }, "No fue posible guardar el documento.");
    }

    private static Result? Validar(AgregarDocumentoRequest r)
    {
        if (!Enum.IsDefined(r.Modulo))
            return Result.Fail("Seleccione un módulo válido.", ResultErrorType.Validation);

        if (r.Modulo == Modulo.Predial && r.PredioId <= 0)
            return Result.Fail("Consulte un predio antes de agregarle un documento.", ResultErrorType.Validation);

        if (r.Modulo == Modulo.IndustriaComercio && (r.ContribuyenteId is null or <= 0))
            return Result.Fail("Consulte un contribuyente antes de agregarle un documento.", ResultErrorType.Validation);

        if (r.TipoDocumentoId <= 0)
            return Result.Fail("Seleccione un tipo de documento válido para el módulo.", ResultErrorType.Validation);

        if (string.IsNullOrWhiteSpace(r.Nombre))
            return Result.Fail("Ingrese el detalle del documento.", ResultErrorType.Validation);

        if (string.IsNullOrWhiteSpace(r.NombreArchivo) || r.Contenido is null)
            return Result.Fail("Seleccione un archivo PDF.", ResultErrorType.Validation);

        return null;
    }
}
