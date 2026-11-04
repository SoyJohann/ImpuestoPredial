using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Coactivos_Predial.Controllers
{
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetFile(string fileName)
        {
            try
            {
                var userIdStr = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdStr, out var userId)) return Forbid();
                await using var master = await _masterFactory.CreateDbContextAsync();
                var municipio = await master.usuarios.Where(u => u.id == userId).Select(u => u.mun).FirstOrDefaultAsync();
                if (string.IsNullOrWhiteSpace(municipio)) return Forbid();

                var baseFolder = _connectionService.GetStorageStringForMunicipio(municipio);
                var safeName = Path.GetFileName(fileName);
                if (!string.Equals(safeName, fileName, StringComparison.Ordinal)) return NotFound();
                var fullBase = Path.GetFullPath(baseFolder);
                var filePath = Path.GetFullPath(Path.Combine(fullBase, safeName));
                var normalizedBase = fullBase.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
                if (!filePath.StartsWith(normalizedBase, OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) return NotFound();
                if (!System.IO.File.Exists(filePath)) return NotFound();

                Response.Headers["X-Content-Type-Options"] = "nosniff";
                Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
                Response.Headers["Cache-Control"] = "private, max-age=0, must-revalidate";
                Response.Headers["Accept-Ranges"] = "bytes";
                var info = new FileInfo(filePath);
                return File(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), "application/pdf", safeName, info.LastWriteTimeUtc, null, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
