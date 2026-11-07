using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.EntityFrameworkCore;

namespace Coactivos_Predial.Shared.Services.DocumentoFiles;

public sealed record DocumentoFile(int Id, string Nombre, string Path);

public interface IDocumentoFileService
{
    Task<DocumentoFile?> GetByIdAsync(int id, CancellationToken cancellationToken);
}

public sealed class DocumentoFileService : IDocumentoFileService
{
    private readonly TenantDbContext _tenantDb;
    private readonly IConnectionService _connectionService;

    public DocumentoFileService(TenantDbContext tenantDb, IConnectionService connectionService)
    {
        _tenantDb = tenantDb;
        _connectionService = connectionService;
    }

    public async Task<DocumentoFile?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var documento = await _tenantDb.Documentos!
            .AsNoTracking()
            .FirstOrDefaultAsync(documento => documento.Id == id, cancellationToken);

        // Un módulo no definido no tiene carpeta de almacenamiento asignada.
        if (documento is null
            || string.IsNullOrWhiteSpace(documento.Ruta)
            || !Enum.IsDefined(documento.Modulo)) return null;

        var baseFolder = _connectionService.GetStorageStringForModulo(documento.Modulo);
        if (!TryResolvePath(baseFolder, documento.Ruta, out var fileName, out var fullPath)) return null;
        return new DocumentoFile(documento.Id, documento.Nombre, fullPath);
    }

    public static bool TryResolvePath(string baseFolder, string storedPath, out string fileName, out string fullPath)
    {
        var normalizedStoredPath = storedPath?.Replace('\\', '/') ?? string.Empty;
        fileName = Path.GetFileName(normalizedStoredPath);
        fullPath = string.Empty;
        if (string.IsNullOrWhiteSpace(baseFolder)
            || string.IsNullOrWhiteSpace(storedPath)
            || fileName is "" or "." or ".."
            || normalizedStoredPath.Split('/').Any(segment => segment is "." or "..")) return false;

        var fullBase = Path.GetFullPath(baseFolder);
        fullPath = Path.GetFullPath(Path.Combine(fullBase, fileName));

        var normalizedBase = fullBase.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        return fullPath.StartsWith(normalizedBase, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }
}
