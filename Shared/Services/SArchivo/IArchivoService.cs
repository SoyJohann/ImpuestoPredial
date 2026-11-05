using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.FileHandler.Responses;

namespace Coactivos_Predial.Shared.Services.FileHandler;

public interface IArchivoService
{
    Task<Result<List<ArchivoDto>>> GetFiles(string name);

    Task<Result> DownloadFileFromServer(string url, string destinationPath);
}
