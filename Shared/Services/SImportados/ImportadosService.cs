using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SImportados;

/// <summary>Acceso a los registros temporales de importación. Solo resuelve la conexión del tenant.</summary>
public partial class ImportadosService : IimportadosService
{
    private readonly ITenantConnectionFactory connFactory;

    public ImportadosService(ITenantConnectionFactory connFactory)
    {
        this.connFactory = connFactory;
    }
}
