using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;

namespace Coactivos_Predial.Shared.Services.SLogin;

public partial class LoginService
{
    public Task<Result> UserRegistrationAsync(Usuarios usuarios) =>
        Task.FromResult(Result.Fail(
            "El registro de nuevos usuarios no está soportado en este servicio. Contacte al administrador.",
            ResultErrorType.NotSupported));
}
