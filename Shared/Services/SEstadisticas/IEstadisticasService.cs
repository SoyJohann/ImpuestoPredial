using Coactivos_Predial.Shared.Services.Common;

namespace Coactivos_Predial.Shared.Services.SEstadisticas;

public sealed record ResumenDocumentosDto(int Predial);

public interface IEstadisticasService
{
    /// <summary>Obtiene el conteo de documentos activos del módulo predial.</summary>
    Task<Result<ResumenDocumentosDto>> GetResumenDocumentosAsync();
}
