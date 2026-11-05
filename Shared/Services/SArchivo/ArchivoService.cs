using Coactivos_Predial.Shared.Models;
using System.Net.Http;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.FileHandler
{
    public class ArchivoService : IArchivoService
    {
        private readonly HttpClient _client;
        private readonly IConnectionService _connectionService; 
        public ArchivoService(HttpClient client, IConnectionService connectionService)
        {
            _client = client;
            _connectionService = connectionService;
        }
        //mirar lo del get de los archivos.
        public async Task<List<Archivo>> GetFiles(string partialFileName)
        {
            var archivos = new List<Archivo>();
            string directoryPath = _connectionService.GetStorageString("FileStorageTemplate");

            try
            {
                if (Path.IsPathRooted(partialFileName))
                {
                    var basePath = Path.GetFullPath(directoryPath);
                    var storedPath = Path.GetFullPath(partialFileName.Trim());
                    var normalizedBase = basePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                        + Path.DirectorySeparatorChar;
                    var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                    if (!storedPath.StartsWith(normalizedBase, comparison) || !File.Exists(storedPath))
                        return archivos;

                    archivos.Add(new Archivo
                    {
                        Name = Path.GetFileName(storedPath),
                        Ruta = storedPath
                    });
                    return archivos;
                }

                // Ensure partialFileName is just the file name
                partialFileName = Path.GetFileName(partialFileName).Trim();

                Console.WriteLine($"Nombre a buscar: {partialFileName}");
                Console.WriteLine($"Directorio: {directoryPath}");

                // List all files in the directory
                string[] allFiles = Directory.GetFiles(directoryPath);
                
                foreach (var file in allFiles)
                {
                    Console.WriteLine(Path.GetFileName(file));
                }

                // Filter the files
                string[] fileEntries = allFiles
                    .Where(file => Path.GetFileName(file).Equals(partialFileName))
                    .ToArray();

                if (fileEntries != null && fileEntries.Length > 0)
                {
                    foreach (string filePath in fileEntries)
                    {
                        var archivo = new Archivo
                        {
                            Name = Path.GetFileName(filePath),
                            Ruta = filePath
                        };

                        Console.WriteLine($"File path: {archivo.Name}");
                        archivos.Add(archivo);
                    }
                }
                else
                {
                    Console.WriteLine($"No existen archivos que coincidan en la ruta: {directoryPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivos: {ex.Message}");
            }

            return archivos;
        }

        public async Task DownloadFileFromServer(string url, string destinationPath)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync(url);
                Console.WriteLine($"url es:{url}");
                response.EnsureSuccessStatusCode();
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(destinationPath, fileBytes);
                Console.WriteLine($"Archivo descargado y guardado en {destinationPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al descargar el archivo: {ex.Message}");
            }
        }
    }
}
