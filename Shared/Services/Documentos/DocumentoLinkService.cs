using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Coactivos_Predial.Shared.AppDbContext;

namespace Coactivos_Predial.Shared.Services.DocumentoFiles;

public sealed record DocumentoLinks(string ViewUrl, string DownloadUrl);

public interface IDocumentoLinkService
{
    Task<DocumentoLinks?> GetLinksAsync(int documentId);
}

public sealed class DocumentoLinkService : IDocumentoLinkService
{
    private readonly AuthenticationStateProvider _authentication;
    private readonly IDbContextFactory<MasterDbContext> _masterFactory;
    private readonly IConfiguration _configuration;

    public DocumentoLinkService(AuthenticationStateProvider authentication, IDbContextFactory<MasterDbContext> masterFactory, IConfiguration configuration)
    {
        _authentication = authentication;
        _masterFactory = masterFactory;
        _configuration = configuration;
    }

    public async Task<DocumentoLinks?> GetLinksAsync(int documentId)
    {
        var user = (await _authentication.GetAuthenticationStateAsync()).User;
        if (user.Identity?.IsAuthenticated != true || !int.TryParse(user.FindFirstValue("UserId"), out var userId)) return null;

        await using var master = await _masterFactory.CreateDbContextAsync();
        var municipality = await master.usuarios.Where(u => u.id == userId).Select(u => u.mun).FirstOrDefaultAsync();
        if (string.IsNullOrWhiteSpace(municipality)) return null;
        var connectionTemplate = _configuration.GetConnectionString("DefaultConnectionTemplate");
        var storageTemplate = _configuration["FileStorageTemplate"];
        if (string.IsNullOrWhiteSpace(connectionTemplate) || string.IsNullOrWhiteSpace(storageTemplate)) return null;

        await using var connection = new NpgsqlConnection(connectionTemplate.Replace("{Database}", municipality, StringComparison.Ordinal));
        await connection.OpenAsync();
        var fileName = await connection.QuerySingleOrDefaultAsync<string>("SELECT archivo FROM documentos WHERE id = @documentId", new { documentId });
        if (string.IsNullOrWhiteSpace(fileName)) return null;
        var baseFolder = storageTemplate.Replace("{Predial}", municipality, StringComparison.Ordinal)
            .Replace("{Database}", municipality, StringComparison.Ordinal);
        if (!DocumentoFileService.TryResolvePath(baseFolder, fileName, out _, out _)) return null;

        return new DocumentoLinks($"/api/documentos/{documentId}/ver", $"/api/documentos/{documentId}/descargar");
    }
}
