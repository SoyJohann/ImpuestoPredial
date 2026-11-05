using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.FileHandler.Responses;

namespace Coactivos_Predial.Shared.Services.FileHandler;

public partial class ArchivoService
{
    public Task<Result<List<ArchivoDto>>> GetFiles(string partialFileName) =>
        ResultExecution.ExecuteAsync(async () =>
        {
            var archivos = new List<ArchivoDto>();
            string directoryPath = connectionService.GetStorageString("FileStorageTemplate");

            if (Path.IsPathRooted(partialFileName))
            {
                var basePath = Path.GetFullPath(directoryPath);
                var storedPath = Path.GetFullPath(partialFileName.Trim());
                var normalizedBase = basePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    + Path.DirectorySeparatorChar;
                var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (!storedPath.StartsWith(normalizedBase, comparison) || !File.Exists(storedPath))
                    return archivos;

                archivos.Add(new ArchivoDto(Path.GetFileName(storedPath), storedPath));
                return archivos;
            }

            partialFileName = Path.GetFileName(partialFileName).Trim();
            var allFiles = Directory.GetFiles(directoryPath);
            var fileEntries = allFiles
                .Where(file => Path.GetFileName(file).Equals(partialFileName))
                .ToArray();

            foreach (string filePath in fileEntries)
            {
                archivos.Add(new ArchivoDto(Path.GetFileName(filePath), filePath));
            }

            return archivos;
        }, "No fue posible consultar los archivos.");
}
