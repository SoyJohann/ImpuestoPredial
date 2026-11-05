using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Coactivos_Predial.Shared.Services.SUser;

/// <summary>Acceso al catálogo maestro de usuarios. Solo resuelve la conexión maestra.</summary>
public partial class UserService : IUserService
{
    private readonly IConfiguration configuration;

    public UserService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private async Task<NpgsqlConnection> OpenMasterAsync()
    {
        var connectionString = configuration.GetConnectionString("MasterConnection")
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No está configurada la conexión maestra.");

        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
