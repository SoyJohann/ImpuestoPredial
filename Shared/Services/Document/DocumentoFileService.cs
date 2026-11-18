using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.Document;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.DocumentoFiles;

public sealed record DocumentoFile(int Id, string NombreDescarga, string Path, string MimeType);

public interface IDocumentoFileService
{
    Task<DocumentoFile?> GetByIdAsync(int id, CancellationToken cancellationToken);
}

public sealed class DocumentoFileService : IDocumentoFileService
{
    private const string DefaultMimeType = "application/pdf";

    private readonly TenantDbContext _tenantDb;
    private readonly IDocumentoStorageService _storage;

    public DocumentoFileService(TenantDbContext tenantDb, IDocumentoStorageService storage)
    {
        _tenantDb = tenantDb;
        _storage = storage;
    }

    public async Task<DocumentoFile?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var documento = await _tenantDb.Documentos!
            .AsNoTracking()
            .FirstOrDefaultAsync(documento =>
                documento.Id == id
                && documento.IsActive
                && documento.EliminadoEn == null, cancellationToken);

        if (documento is null || !_storage.TryResolver(documento, out var fullPath))
            return null;

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

    /// <summary>Compatibilidad con pruebas y llamadas heredadas.</summary>
    public static bool TryResolvePath(string baseFolder, string storedPath, out string fileName, out string fullPath) =>
        DocumentoStoragePaths.TryResolveLegacyPath(baseFolder, storedPath, out fileName, out fullPath);
}
