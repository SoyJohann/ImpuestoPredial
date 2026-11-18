using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.Document.Interfaces;
using Coactivos_Predial.Shared.Services.Document.Reponses;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.Document;

public sealed class DocumentoFileService(TenantDbContext tenantDb, IDocumentoStorageService storage)
    : IDocumentoFileService
{
    private const string DefaultMimeType = "application/pdf";

    public async Task<DocumentoFile?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            if (tenantDb is null)
            {
                Console.WriteLine("GetByIdAsync: tenantDb is null");
                return null;
            }

            if (tenantDb.Documentos is null)
            {
                Console.WriteLine("GetByIdAsync: tenantDb.Documentos is null");
                return null;
            }

            var documento = await tenantDb.Documentos
                .AsNoTracking()
                .FirstOrDefaultAsync(documento =>
                    documento.Id == id
                    && documento.IsActive
                    && documento.EliminadoEn == null, cancellationToken);

            if (documento is null)
            {
                Console.WriteLine($"GetByIdAsync: documento {id} not found or inactive");
                return null;
            }

            if (storage is null)
            {
                Console.WriteLine("GetByIdAsync: storage is null");
                return null;
            }

            if (!storage.TryResolver(documento, out var fullPath))
            {
                Console.WriteLine($"GetByIdAsync: storage.TryResolver returned false for documento {id}");
                return null;
            }

            // El nombre de descarga es el original saneado; los documentos heredados
            // caen al nombre del archivo almacenado en Ruta.
            var nombreDescarga = !string.IsNullOrWhiteSpace(documento.NombreArchivo)
                ? documento.NombreArchivo
                : Path.GetFileName((documento.Ruta ?? string.Empty).Replace('\\', '/'));

            if (string.IsNullOrWhiteSpace(nombreDescarga))
                nombreDescarga = Path.GetFileName(fullPath);

            var mimeType = string.IsNullOrWhiteSpace(documento.MimeType) ? DefaultMimeType : documento.MimeType;
            return new DocumentoFile(documento.Id, nombreDescarga, fullPath, mimeType);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetByIdAsync error: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            return null;
        }
    }

    /// <summary>Compatibilidad con pruebas y llamadas heredadas.</summary>
    public static bool TryResolvePath(string baseFolder, string storedPath, out string fileName, out string fullPath) =>
        DocumentoStoragePaths.TryResolveLegacyPath(baseFolder, storedPath, out fileName, out fullPath);
}
