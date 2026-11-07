using Coactivos_Predial.Shared.Services.Document.Enums;
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

        Assert.Equal("/home/johan/Cimitarra/Predial/", service.GetStorageStringForTipo(Modulo.Predial));
    }

    [Fact]
    public void BuildsIndustriaComercioPathForCurrentTenant()
    {
        var service = new ConnectionService(Configuration(), new TenantProviderStub("Cimitarra"));

        Assert.Equal("/home/johan/Cimitarra/IndustriaComercio/", service.GetStorageStringForTipo(Modulo.IndustriaComercio));
    }

    [Fact]
    public void TenantScopedServicesDoNotShareMunicipalityPaths()
    {
        var configuration = Configuration();
        var cimitarra = new ConnectionService(configuration, new TenantProviderStub("Cimitarra"));
        var otroMunicipio = new ConnectionService(configuration, new TenantProviderStub("OtroMunicipio"));

        Assert.Equal("/home/johan/Cimitarra/Predial/", cimitarra.GetStorageStringForTipo(Modulo.Predial));
        Assert.Equal("/home/johan/OtroMunicipio/Predial/", otroMunicipio.GetStorageStringForTipo(Modulo.Predial));
    }

    [Fact]
    public void UsesTenantNameDirectlyAsStorageFolder()
    {
        var service = new ConnectionService(Configuration(), new TenantProviderStub("Cimitarra"));

        Assert.Equal("/home/johan/Cimitarra/Predial/", service.GetStorageStringForTipo(Modulo.Predial));
    }

    [Fact]
    public void RejectsUnknownTipoAndMunicipalityPathSegments()
    {
        var service = new ConnectionService(Configuration(), new TenantProviderStub("../OtroMunicipio"));

        Assert.Equal(string.Empty, service.GetStorageStringForTipo(Modulo.Predial));
        Assert.Equal(string.Empty, service.GetStorageStringForMunicipio("Cimitarra", (Modulo)99));
    }

    private sealed class TenantProviderStub(string? municipio) : ITenantProvider
    {
        public string? Municipio { get; } = municipio;
        public Task InitializeAsync() => Task.CompletedTask;
    }
}
