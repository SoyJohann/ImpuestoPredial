using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.DocumentoFiles;

public sealed record DocumentoFile(int Id, string CodigoPredial, string Nombre, string Path);
public sealed class DocumentoAccessDeniedException : Exception { }

public interface IDocumentoFileService
{
    Task<DocumentoFile?> GetForCurrentUserAsync(int id, CancellationToken cancellationToken);
}

public sealed class DocumentoFileService : IDocumentoFileService
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly IDbContextFactory<MasterDbContext> _masterFactory;
    private readonly IConfiguration _configuration;

    public DocumentoFileService(IHttpContextAccessor httpContext, IDbContextFactory<MasterDbContext> masterFactory, IConfiguration configuration)
    {
        _httpContext = httpContext;
        _masterFactory = masterFactory;
        _configuration = configuration;
    }

    public async Task<DocumentoFile?> GetForCurrentUserAsync(int id, CancellationToken cancellationToken)
    {
        var user = _httpContext.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true || !int.TryParse(user.FindFirstValue("UserId"), out var userId))
            throw new DocumentoAccessDeniedException();

        await using var master = await _masterFactory.CreateDbContextAsync(cancellationToken);
        var municipality = await master.usuarios.Where(u => u.id == userId).Select(u => u.mun).FirstOrDefaultAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(municipality)) throw new DocumentoAccessDeniedException();

        var connectionTemplate = _configuration.GetConnectionString("DefaultConnectionTemplate");
        var storageTemplate = _configuration["FileStorageTemplate"];
        if (string.IsNullOrWhiteSpace(connectionTemplate) || string.IsNullOrWhiteSpace(storageTemplate)) return null;

        var connectionString = connectionTemplate.Replace("{Database}", municipality, StringComparison.Ordinal);
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        var document = await connection.QuerySingleOrDefaultAsync<DocumentRow>(new CommandDefinition(
            "SELECT id, codigo_predial, archivo FROM documentos WHERE id = @id", new { id }, cancellationToken: cancellationToken));
        if (document is null || string.IsNullOrWhiteSpace(document.Archivo)) return null;

        // Stored document names must be simple file names. Reject paths, including traversal attempts.
        var baseFolder = storageTemplate.Replace("{Predial}", municipality, StringComparison.Ordinal)
            .Replace("{Database}", municipality, StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(baseFolder)) return null;
        if (!TryResolvePath(baseFolder, document.Archivo, out var fileName, out var fullPath)) return null;
        return new DocumentoFile(document.Id, document.CodigoPredial ?? string.Empty, fileName, fullPath);
    }

    public static bool TryResolvePath(string baseFolder, string storedName, out string fileName, out string fullPath)
    {
        fileName = System.IO.Path.GetFileName(storedName);
        fullPath = string.Empty;
        if (!string.Equals(fileName, storedName, StringComparison.Ordinal) || fileName is "." or "..") return false;
        var fullBase = System.IO.Path.GetFullPath(baseFolder);
        fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(fullBase, fileName));
        var normalizedBase = fullBase.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar)
            + System.IO.Path.DirectorySeparatorChar;
        return fullPath.StartsWith(normalizedBase, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }

    private sealed class DocumentRow
    {
        public int Id { get; set; }
        public string? CodigoPredial { get; set; }
        public string? Archivo { get; set; }
    }
}
