using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.Spersuasivo;

/// <summary>Acceso a los procesos persuasivos. Solo resuelve la conexión del tenant.</summary>
public partial class PersuasivoService : IPersuasivoService
{
    private readonly ITenantConnectionFactory connFactory;

    public PersuasivoService(ITenantConnectionFactory connFactory)
    {
        this.connFactory = connFactory;
    }
}
