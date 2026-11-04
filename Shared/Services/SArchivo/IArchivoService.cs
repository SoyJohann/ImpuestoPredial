using Coactivos_Predial.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coactivos_Predial.Shared.Services.FileHandler
{
    public interface IArchivoService
    {

        Task<List<Archivo>> GetFiles(string name);
        Task DownloadFileFromServer(string url, string destinationPath);

    }
}
