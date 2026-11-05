using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SCoactivos;

/// <summary>Acceso a los procesos coactivos. Solo resuelve la conexión del tenant.</summary>
public partial class CoactivosService : ICoactivosService
{
    private readonly ITenantConnectionFactory connFactory;

    public CoactivosService(ITenantConnectionFactory connFactory)
    {
        this.connFactory = connFactory;
    }
}
