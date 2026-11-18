using System.Security.Cryptography;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.Document;

public sealed record DocumentoGuardado(
    string StorageKey,
    string RutaCompleta,
    long Tamano,
    string HashSha256,
    string MimeType);

public interface IDocumentoStorageService
{
    string CrearStorageKey();
    string ObtenerCarpetaBase(Modulo modulo);
    string ResolverRuta(Modulo modulo, string storageKey);
    bool TryResolver(Documentos documento, out string rutaCompleta);
    Task<DocumentoGuardado> GuardarAsync(Modulo modulo, Stream contenido, CancellationToken cancellationToken);
    void EliminarArchivo(string rutaCompleta);
}

/// <summary>
/// Única puerta de entrada al disco documental. Decide la ruta a partir de la
/// clave opaca y valida el contenido real del PDF antes de guardarlo.
/// </summary>
public sealed class DocumentoStorageService : IDocumentoStorageService
{
    public const long MaxFileSize = 4 * 1024 * 1024;
    private const string PdfMimeType = "application/pdf";

    private readonly IConnectionService _connectionService;
    private readonly ILogger<DocumentoStorageService> _logger;

    public DocumentoStorageService(IConnectionService connectionService, ILogger<DocumentoStorageService> logger)
    {
        _connectionService = connectionService;
        _logger = logger;
    }

    public string CrearStorageKey() => DocumentoStoragePaths.CreateStorageKey();

    public string ObtenerCarpetaBase(Modulo modulo) =>
        _connectionService.GetStorageStringForModulo(modulo).Trim();

    public string ResolverRuta(Modulo modulo, string storageKey) =>
        DocumentoStoragePaths.GetFullPath(ObtenerCarpetaBase(modulo), storageKey);

    public bool TryResolver(Documentos documento, out string rutaCompleta)
    {
        rutaCompleta = string.Empty;

        if (!Enum.IsDefined(documento.Modulo))
        {
            _logger.LogWarning("Documento {DocumentoId} tiene un módulo desconocido {Modulo}.", documento.Id, (int)documento.Modulo);
            return false;
        }

        var baseFolder = ObtenerCarpetaBase(documento.Modulo);
        if (string.IsNullOrWhiteSpace(baseFolder))
        {
            _logger.LogWarning("No hay carpeta de almacenamiento configurada para el módulo {Modulo}.", documento.Modulo);
            return false;
        }

        // Si existe StorageKey, es la única fuente válida: no se cae a Ruta.
        if (!string.IsNullOrWhiteSpace(documento.StorageKey))
        {
            try
            {
                rutaCompleta = DocumentoStoragePaths.GetFullPath(baseFolder, documento.StorageKey);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                _logger.LogWarning(ex, "StorageKey inválido en el documento {DocumentoId}.", documento.Id);
                return false;
            }

            if (File.Exists(rutaCompleta))
                return true;

            _logger.LogWarning(
                "No se encontró el archivo del documento {DocumentoId} para la StorageKey {StorageKey}.",
                documento.Id,
                documento.StorageKey);
            return false;
        }

        if (string.IsNullOrWhiteSpace(documento.Ruta))
        {
            _logger.LogWarning("El documento {DocumentoId} no tiene StorageKey ni Ruta.", documento.Id);
            return false;
        }

        if (!DocumentoStoragePaths.TryResolveLegacyPath(baseFolder, documento.Ruta, out _, out rutaCompleta))
        {
            _logger.LogWarning("La ruta heredada del documento {DocumentoId} no es válida.", documento.Id);
            return false;
        }

        if (File.Exists(rutaCompleta))
            return true;

        _logger.LogWarning(
            "No se encontró el archivo heredado del documento {DocumentoId} en {Ruta}.",
            documento.Id,
            rutaCompleta);
        return false;
    }

    public async Task<DocumentoGuardado> GuardarAsync(Modulo modulo, Stream contenido, CancellationToken cancellationToken)
    {
        if (contenido is null)
            throw new ArgumentException("Seleccione un archivo PDF.", nameof(contenido));

        var baseFolder = ObtenerCarpetaBase(modulo);
        if (string.IsNullOrWhiteSpace(baseFolder))
            throw new InvalidOperationException("No hay carpeta de almacenamiento configurada para el módulo.");

        var storageKey = CrearStorageKey();
        var fullPath = DocumentoStoragePaths.GetFullPath(baseFolder, storageKey);

        await using var buffer = new MemoryStream();
        var chunk = new byte[81920];
        long total = 0;
        int read;

        while ((read = await contenido.ReadAsync(chunk, cancellationToken)) > 0)
        {
            total += read;
            if (total > MaxFileSize)
                throw new ArgumentException("El PDF debe pesar hasta 4 MB.", nameof(contenido));

            await buffer.WriteAsync(chunk.AsMemory(0, read), cancellationToken);
        }

        if (total == 0)
            throw new ArgumentException("El archivo está vacío.", nameof(contenido));

        var bytes = buffer.ToArray();
        if (bytes.Length < 5 || !bytes.AsSpan(0, 5).SequenceEqual("%PDF-"u8))
            throw new ArgumentException("El archivo no es un PDF válido.", nameof(contenido));

        var directory = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(directory);
        await File.WriteAllBytesAsync(fullPath, bytes, cancellationToken);

        var hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        return new DocumentoGuardado(storageKey, fullPath, bytes.LongLength, hash, PdfMimeType);
    }

    public void EliminarArchivo(string rutaCompleta)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(rutaCompleta) && File.Exists(rutaCompleta))
                File.Delete(rutaCompleta);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No fue posible eliminar el archivo {Ruta}.", rutaCompleta);
        }
    }
}
