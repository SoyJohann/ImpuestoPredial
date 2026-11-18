using Coactivos_Predial.Shared.Services.Document;
using Xunit;

namespace Coactivos_Predial.Tests;

public sealed class DocumentoStoragePathsTests
{
    [Fact]
    public void DistributesFilesInTwoLevels()
    {
        var key = "abcdef0123456789abcdef0123456789.pdf";
        var relative = DocumentoStoragePaths.GetRelativePath(key);

        Assert.Equal(Path.Combine("ab", "cd", key), relative);
    }

    [Fact]
    public void RejectsStorageKeysWithTraversal()
    {
        Assert.Throws<ArgumentException>(() => DocumentoStoragePaths.GetRelativePath("../fuera.pdf"));
        Assert.Throws<ArgumentException>(() => DocumentoStoragePaths.GetRelativePath("carpeta/archivo.pdf"));
    }

    [Fact]
    public void ResolvesLegacyAbsolutePathByFileNameInsideSectorFolder()
    {
        var baseFolder = Path.Combine(Path.GetTempPath(), "predial-documents", "Cimitarra", "Predial");
        var storedPath = Path.Combine(Path.GetTempPath(), "old-storage", "Cimitarra", "recibo.pdf");

        Assert.True(DocumentoStoragePaths.TryResolveLegacyPath(baseFolder, storedPath, out var fileName, out var resolved));
        Assert.Equal("recibo.pdf", fileName);
        Assert.Equal(Path.GetFullPath(Path.Combine(baseFolder, fileName)), resolved);
        Assert.StartsWith(Path.GetFullPath(baseFolder) + Path.DirectorySeparatorChar, resolved);
    }

    [Fact]
    public void SanitizesOriginalNameWithoutUsingItAsPath()
    {
        var sanitized = DocumentoStoragePaths.SanitizeFileName("../../recibo:2026.pdf");

        Assert.Equal("recibo_2026.pdf", sanitized);
        Assert.DoesNotContain('/', sanitized);
        Assert.DoesNotContain('\\', sanitized);
    }

    [Fact]
    public void AddsPdfExtensionWhenMissing()
    {
        Assert.Equal("recibo.pdf", DocumentoStoragePaths.SanitizeFileName("recibo"));
    }
}
