using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Coactivos_Predial.Shared.AppDbContext;

/// <summary>
/// Proporciona a dotnet ef el contexto apuntado a la base nueva de code-first,
/// sin depender del municipio autenticado en tiempo de ejecución.
/// </summary>
public sealed class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    private const string DatabaseName = "Cimitarra";

    public TenantDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionTemplate = configuration.GetConnectionString("DefaultConnectionTemplate")
            ?? throw new InvalidOperationException("Falta configurar DefaultConnectionTemplate.");
        var connectionString = connectionTemplate.Replace("{Database}", DatabaseName, StringComparison.Ordinal);
        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new TenantDbContext(options);
    }
}
