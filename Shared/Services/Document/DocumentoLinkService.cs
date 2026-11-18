using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.DocumentoFiles;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.Document;

public sealed record DocumentoLinks(string ViewUrl, string DownloadUrl);

public sealed class DocumentoLinkService(TenantDbContext context, IDocumentoStorageService storage) : IDocumentoLinkService
{
    public async Task<DocumentoLinks?> GetLinksAsync(int documentId)
    {
        if (context.Documentos is null)
        {
            return null;
        }

        var documento = await context.Documentos
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == documentId
                && s.IsActive
                && s.EliminadoEn == null);

        if (documento is null || !storage.TryResolver(documento, out _))
        {
            return null;
        }

        return new DocumentoLinks(
            $"/api/documentos/{documentId}/ver",
            $"/api/documentos/{documentId}/descargar"
        );
    }
}
