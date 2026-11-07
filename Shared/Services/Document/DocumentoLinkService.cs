using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.DocumentoFiles;
using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.Document;

public sealed record DocumentoLinks(string ViewUrl, string DownloadUrl);

public sealed class DocumentoLinkService(TenantDbContext context, IConnectionService connectionService) : IDocumentoLinkService
{
    public async Task<DocumentoLinks?> GetLinksAsync(int documentId)
    {
        // 1. Consultar el documento directamente en la BD del tenant actual
        if (context.Documentos is null)
        {
            return null;
        }

        var documento = await context.Documentos
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == documentId);

        // Un módulo no definido no tiene carpeta de almacenamiento asignada.
        if (documento == null
            || string.IsNullOrWhiteSpace(documento.Ruta)
            || !Enum.IsDefined(documento.Modulo))
        {
            return null;
        }
        var baseFolder = connectionService.GetStorageStringForModulo(documento.Modulo);
        if (!DocumentoFileService.TryResolvePath(baseFolder, documento.Ruta.Trim(), out _, out var filePath)
            || !File.Exists(filePath))
        {
            return null;
        }

        return new DocumentoLinks(
            $"/api/documentos/{documentId}/ver",
            $"/api/documentos/{documentId}/descargar"
        );
    }
}
