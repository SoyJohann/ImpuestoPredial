using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SParametros;

/// <summary>Acceso a los parámetros del municipio. Solo resuelve la conexión del tenant.</summary>
public partial class ParametroService : IParametroService
{
    private readonly ITenantConnectionFactory connFactory;

    public ParametroService(ITenantConnectionFactory connFactory)
    {
        this.connFactory = connFactory;
    }
}
