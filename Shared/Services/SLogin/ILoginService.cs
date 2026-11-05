using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.Common;

namespace Coactivos_Predial.Shared.Services.SLogin;

public interface ILoginService
{
    Task<Result> UserLoginAsync(string username, string pass);

    bool ValidatePasswordHash(string password, string dbpassword);

    Task<Result> UserRegistrationAsync(Usuarios usuarios);
}
