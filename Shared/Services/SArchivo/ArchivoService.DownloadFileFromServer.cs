using Coactivos_Predial.Shared.Services.Common;

namespace Coactivos_Predial.Shared.Services.FileHandler;

public partial class ArchivoService
{
    public Task<Result> DownloadFileFromServer(string url, string destinationPath) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(destinationPath, fileBytes);
        }, "No fue posible descargar el archivo.");
}
