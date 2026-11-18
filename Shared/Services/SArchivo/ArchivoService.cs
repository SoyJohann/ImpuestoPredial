using Coactivos_Predial.Shared.Services.FileHandler;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SArchivo;

/// <summary>Acceso a archivos del almacenamiento municipal.</summary>
public partial class ArchivoService(HttpClient client, IConnectionService connectionService) : IArchivoService;
