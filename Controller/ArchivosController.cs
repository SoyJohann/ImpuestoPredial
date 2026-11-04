using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Coactivos_Predial.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArchivosController : ControllerBase
    {
        private readonly IConnectionService _connectionService;
        private readonly IDbContextFactory<MasterDbContext> _masterFactory;

        public ArchivosController(IConnectionService connectionService, IDbContextFactory<MasterDbContext> masterFactory)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
            _masterFactory = masterFactory ?? throw new ArgumentNullException(nameof(masterFactory));
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetFile(string fileName, [FromQuery] string? mun = null)
        {
            try
            {
                string? municipio = mun;
                if (string.IsNullOrEmpty(municipio))
                {
                    var userIdStr = User.FindFirst("UserId")?.Value;
                    if (int.TryParse(userIdStr, out var userId))
                    {
                        await using var master = await _masterFactory.CreateDbContextAsync();
                        municipio = await master.usuarios
                            .Where(u => u.id == userId)
                            .Select(u => u.mun)
                            .FirstOrDefaultAsync();
                    }
                }

                var baseFolder = _connectionService.GetStorageStringForMunicipio(municipio);
                var filePath = Path.Combine(baseFolder, fileName);

                Console.WriteLine($"el filepath en el controller es : {filePath}");
                if (System.IO.File.Exists(filePath))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;

                    Console.WriteLine($"Archivo {fileName} leído correctamente.");
                    return File(memory, "application/pdf", fileName);
                }
                else
                {
                    Console.WriteLine($"El archivo {fileName} no existe en el servidor.");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
