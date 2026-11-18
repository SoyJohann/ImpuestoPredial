using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.Document.Interfaces;
using Coactivos_Predial.Shared.Services.Document.Reponses;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.Document;

public sealed class DocumentoLinkService(TenantDbContext context, IDocumentoStorageService storage) : IDocumentoLinkService
{
    public async Task<DocumentoLinksDto?> GetLinksAsync(int documentId)
    {
        try
        {
            if (context is null)
            {
                Console.WriteLine("GetLinksAsync: context is null");
                return null;
            }

            if (context.Documentos is null)
            {
                Console.WriteLine("GetLinksAsync: context.Documentos is null");
                return null;
            }

            var documento = await context.Documentos
                .AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.Id == documentId
                    && s.IsActive
                    && s.EliminadoEn == null);

            if (documento is null)
            {
                Console.WriteLine($"GetLinksAsync: documento {documentId} not found or inactive");
                return null;
            }

            if (storage is null)
            {
                Console.WriteLine("GetLinksAsync: storage is null");
                return null;
            }

            if (!storage.TryResolver(documento, out _))
            {
                Console.WriteLine($"GetLinksAsync: storage.TryResolver returned false for documento {documentId}");
                return null;
            }

            return new DocumentoLinksDto(
                $"/api/documentos/{documentId}/ver",
                $"/api/documentos/{documentId}/descargar"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetLinksAsync error: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return null;
        }
    }
}