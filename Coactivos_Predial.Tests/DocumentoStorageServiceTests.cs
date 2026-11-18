using System.Security.Cryptography;
using System.Text;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Document;
using Coactivos_Predial.Shared.Services.Document.Enums;
using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Coactivos_Predial.Tests;

public sealed class DocumentoStorageServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "sgd-" + Guid.NewGuid().ToString("N"));
    private readonly DocumentoStorageService _service;

    public DocumentoStorageServiceTests()
    {
        Directory.CreateDirectory(_root);
        _service = new DocumentoStorageService(
            new ConnectionStub(_root),
            NullLogger<DocumentoStorageService>.Instance);
    }

    [Fact]
    public async Task SavesPdfWithHashAndTwoLevelPath()
    {
        var bytes = Encoding.ASCII.GetBytes("%PDF-1.4 contenido de prueba");

        var guardado = await _service.GuardarAsync(Modulo.Predial, new MemoryStream(bytes), CancellationToken.None);

        Assert.EndsWith(".pdf", guardado.StorageKey);
        Assert.True(File.Exists(guardado.RutaCompleta));
        Assert.Equal(bytes.LongLength, guardado.Size);
        Assert.Equal(Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant(), guardado.HashSha256);
        Assert.Equal("application/pdf", guardado.MimeType);
        Assert.Contains(Path.Combine(guardado.StorageKey[..2], guardado.StorageKey[2..4]), guardado.RutaCompleta);
    }

    [Fact]
    public async Task RejectsFilesThatAreNotPdf()
    {
        var bytes = Encoding.ASCII.GetBytes("no soy un pdf");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GuardarAsync(Modulo.Predial, new MemoryStream(bytes), CancellationToken.None));
    }

    [Fact]
    public void MissingStorageKeyDoesNotFallBackToLegacyRuta()
    {
        File.WriteAllText(Path.Combine(_root, "heredado.pdf"), "%PDF-1.4");
        var documento = new Documentos
        {
            Id = 1,
            Modulo = Modulo.Predial,
            StorageKey = "archivo-inexistente.pdf",
            Ruta = "heredado.pdf"
        };

        Assert.False(_service.TryResolver(documento, out _));
    }

    [Fact]
    public void LegacyRutaResolvesWhenStorageKeyIsNull()
    {
        File.WriteAllText(Path.Combine(_root, "heredado.pdf"), "%PDF-1.4");
        var documento = new Documentos
        {
            Id = 2,
            Modulo = Modulo.Predial,
            Ruta = "heredado.pdf"
        };

        Assert.True(_service.TryResolver(documento, out var ruta));
        Assert.Equal(Path.Combine(_root, "heredado.pdf"), ruta);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    private sealed class ConnectionStub(string folder) : IConnectionService
    {
        public string GetConnectionString(string municipio) => string.Empty;
        public string GetStorageStringForModulo(Modulo tipo) => folder;
        public string GetStorageStringForTipo(Modulo tipo) => folder;
        public string GetStorageStringForMunicipio(string? municipio, Modulo tipo) => folder;
    }
}
