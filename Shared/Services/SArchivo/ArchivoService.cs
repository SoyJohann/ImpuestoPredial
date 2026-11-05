using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.FileHandler;

/// <summary>Acceso a archivos del almacenamiento municipal.</summary>
public partial class ArchivoService : IArchivoService
{
    private readonly HttpClient client;
    private readonly IConnectionService connectionService;

    public ArchivoService(HttpClient client, IConnectionService connectionService)
    {
        this.client = client;
        this.connectionService = connectionService;
    }
}
