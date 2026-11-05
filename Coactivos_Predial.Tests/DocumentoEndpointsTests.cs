using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Coactivos_Predial.Shared.Services.DocumentoFiles;
using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Coactivos_Predial.Tests;

public sealed class DocumentoEndpointsTests : IClassFixture<DocumentosFactory>
{
    private readonly HttpClient _client;
    private readonly DocumentosFactory _factory;

    public DocumentoEndpointsTests(DocumentosFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ServesFullPdf()
    {
        using var request = AuthorizedRequest("/api/documentos/1/ver");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(_factory.PdfBytes, await response.Content.ReadAsByteArrayAsync());
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
    }

    [Fact]
    public async Task ServesRequestedByteRange()
    {
        using var request = AuthorizedRequest("/api/documentos/1/ver");
        request.Headers.Range = new RangeHeaderValue(10, 19);
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.PartialContent, response.StatusCode);
        Assert.Equal("bytes 10-19/1024", response.Content.Headers.ContentRange?.ToString());
        Assert.Equal(10, (await response.Content.ReadAsByteArrayAsync()).Length);
    }

    [Fact]
    public async Task DownloadUsesAttachmentDisposition()
    {
        using var request = AuthorizedRequest("/api/documentos/1/descargar");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("attachment", response.Content.Headers.ContentDisposition?.DispositionType);
        Assert.False(string.IsNullOrWhiteSpace(response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName));
    }

    [Fact]
    public async Task RejectsInvalidRange()
    {
        using var request = AuthorizedRequest("/api/documentos/1/ver");
        request.Headers.Range = new RangeHeaderValue(2000, 2050);
        var response = await _client.SendAsync(request);
        Assert.Equal((HttpStatusCode)416, response.StatusCode);
    }

    [Fact]
    public async Task RequiresAuthentication()
    {
        var response = await _client.GetAsync("/api/documentos/1/ver");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RejectsAuthenticatedUserWithoutTenantClaim()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/documentos/1/ver");
        request.Headers.Add("X-Test-Auth", "forbidden");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsNotFoundForMissingDocument()
    {
        using var request = AuthorizedRequest("/api/documentos/404/ver");
        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public void RefusesStoredTraversalName()
    {
        var baseFolder = Path.Combine(Path.GetTempPath(), "predial-documents");
        Assert.False(DocumentoFileService.TryResolvePath(baseFolder, "../outside.pdf", out _, out _));
    }

    [Fact]
    public void ResolvesLegacyAbsolutePathByFileNameInsideSectorFolder()
    {
        var baseFolder = Path.Combine(Path.GetTempPath(), "predial-documents", "Cimitarra", "Predial");
        var storedPath = Path.Combine(Path.GetTempPath(), "old-storage", "Cimitarra", "recibo.pdf");
        Assert.True(DocumentoFileService.TryResolvePath(baseFolder, storedPath, out var fileName, out var resolved));
        Assert.Equal("recibo.pdf", fileName);
        Assert.Equal(Path.GetFullPath(Path.Combine(baseFolder, fileName)), resolved);
        Assert.StartsWith(Path.GetFullPath(baseFolder) + Path.DirectorySeparatorChar, resolved);
    }

    private static HttpRequestMessage AuthorizedRequest(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-Test-Auth", "valid");
        return request;
    }
}

public sealed class DocumentosFactory : WebApplicationFactory<Program>
{
    public byte[] PdfBytes { get; } = Enumerable.Range(0, 1024).Select(i => (byte)(i % 251)).ToArray();
    private readonly string _pdfPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".pdf");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        File.WriteAllBytes(_pdfPath, PdfBytes);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDocumentoFileService>();
            services.AddSingleton<IDocumentoFileService>(new FakeDocumentoFileService(_pdfPath));
            services.RemoveAll<ITenantProvider>();
            services.AddScoped<ITenantProvider, FakeTenantProvider>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultForbidScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", _ => { });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && File.Exists(_pdfPath)) File.Delete(_pdfPath);
    }
}

public sealed class FakeDocumentoFileService(string path) : IDocumentoFileService
{
    public Task<DocumentoFile?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult<DocumentoFile?>(id == 1 ? new DocumentoFile(1, "test.pdf", path) : null);
    }
}

public sealed class FakeTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _context;
    public FakeTenantProvider(IHttpContextAccessor context) => _context = context;
    public string? Municipio => _context.HttpContext?.User.FindFirst("UserId") is null ? null : "test";
    public Task InitializeAsync() => Task.CompletedTask;
}

public sealed class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-Auth", out var auth)) return Task.FromResult(AuthenticateResult.NoResult());
        var claims = auth == "valid"
            ? new[] { new Claim(ClaimTypes.Name, "test"), new Claim("UserId", "1") }
            : new[] { new Claim(ClaimTypes.Name, "no-tenant") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
    }
}
