using Coactivos_Predial.Shared.Services.Common;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

public partial class PropietariosService
{
    public Task<Result> DeleteInfractores(int id) =>
        Task.FromResult(Result.Fail(
            "La eliminación de propietarios no está implementada.",
            ResultErrorType.NotSupported));
}
