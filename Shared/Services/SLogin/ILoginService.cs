
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.Services.SLogin
{
    public interface ILoginService
    {
        Task<int> UserLoginAsync(string username, string pass);
        bool ValidatePasswordHash(string password, string dbpassword);
        Task<(bool Success, string Message)> UserRegistrationAsync(Usuarios usuarios);

    }
}
