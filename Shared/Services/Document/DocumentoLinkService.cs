using Coactivos_Predial.Shared.AppDbContext;
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
            .Include(documento => documento.TipoDocumento)
                .ThenInclude(tipo => tipo.Sector)
            .FirstOrDefaultAsync(s => s.Id == documentId);

        if (documento == null
            || string.IsNullOrWhiteSpace(documento.Ruta)
            || documento.TipoDocumento?.Sector is null)
        {
            return null;
        }
        var baseFolder = connectionService.GetStorageStringForSector(documento.TipoDocumento.Sector.Codigo);
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
