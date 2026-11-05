using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SParametros.Responses;

namespace Coactivos_Predial.Shared.Services.SParametros;

public interface IParametroService
{
    Task<Result<List<ParametroDto>>> GetAll();

    Task<Result> EditParametros(int id, ParametroDto parametros);

    Task<Result> EditEstados(EstadoProcesoDto estado);

    Task<Result<ParametroDto?>> GetParametroById(int id);

    Task<Result<List<EstadoProcesoDto>>> Getlist();
}
