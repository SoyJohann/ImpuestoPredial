using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coactivos_Predial.Controller
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ArchivosController : ControllerBase
    {
        private readonly IConnectionService _connectionService;

        public ArchivosController(IConnectionService connectionService)
        {
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            try
            {
                var baseFolder = _connectionService.GetStorageString("FileStorageTemplate");
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
