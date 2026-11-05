using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Services.SPropietarios.Responses;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result<PropietarioDto?>> GetInfractores(int id) =>
        Task.FromResult(Result<PropietarioDto?>.Fail(
            "La consulta de propietarios por identificador no está implementada.",
            ResultErrorType.NotSupported));
}
