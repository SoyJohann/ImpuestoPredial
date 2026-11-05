using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Coactivos_Predial.Tests;

public sealed class ConnectionServiceTests
{
    private static IConfiguration Configuration() => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["FileStorageTemplate"] = "/home/johan/{Municipio}/{Sector}/"
        })
        .Build();

    [Fact]
    public void BuildsPredialPathForCurrentTenant()
    {
        var service = new ConnectionService(Configuration(), new TenantProviderStub("Cimitarra"));

        Assert.Equal("/home/johan/Cimitarra/Predial/", service.GetStorageStringForSector("PREDIAL"));
    }

    [Fact]
    public void BuildsIndustriaComercioPathForCurrentTenant()
    {
        var service = new ConnectionService(Configuration(), new TenantProviderStub("Cimitarra"));

        Assert.Equal("/home/johan/Cimitarra/IndustriaComercio/", service.GetStorageStringForSector("INDUSTRIA_COMERCIO"));
    }

    [Fact]
    public void TenantScopedServicesDoNotShareMunicipalityPaths()
    {
        var configuration = Configuration();
        var cimitarra = new ConnectionService(configuration, new TenantProviderStub("Cimitarra"));
        var otroMunicipio = new ConnectionService(configuration, new TenantProviderStub("OtroMunicipio"));

        Assert.Equal("/home/johan/Cimitarra/Predial/", cimitarra.GetStorageStringForSector("PREDIAL"));
        Assert.Equal("/home/johan/OtroMunicipio/Predial/", otroMunicipio.GetStorageStringForSector("PREDIAL"));
    }

    [Fact]
    public void RemovesPredialDatabasePrefixFromStorageFolderName()
    {
        var service = new ConnectionService(Configuration(), new TenantProviderStub("Predial_Cimitarra"));

        Assert.Equal("/home/johan/Cimitarra/Predial/", service.GetStorageStringForSector("PREDIAL"));
    }

    [Fact]
    public void RejectsUnknownSectorAndMunicipalityPathSegments()
    {
        var service = new ConnectionService(Configuration(), new TenantProviderStub("../OtroMunicipio"));

        Assert.Equal(string.Empty, service.GetStorageStringForSector("PREDIAL"));
        Assert.Equal(string.Empty, service.GetStorageStringForMunicipio("Cimitarra", "DESCONOCIDO"));
    }

    private sealed class TenantProviderStub(string? municipio) : ITenantProvider
    {
        public string? Municipio { get; } = municipio;
        public Task InitializeAsync() => Task.CompletedTask;
    }
}
