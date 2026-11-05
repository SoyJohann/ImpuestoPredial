using System.Security.Claims;
using Coactivos_Predial.Shared.Services.Common;
using Coactivos_Predial.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Coactivos_Predial.Shared.Services.SLogin;

public partial class LoginService
{
    public async Task<Result> UserLoginAsync(string username, string pass)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(pass))
            return Result.Fail("Ingrese el usuario y la clave.", ResultErrorType.Validation);

        try
        {
            Usuarios? user = await masterContext.usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(account =>
                    account.Login == username.Trim().ToLowerInvariant() && account.Habilitado);

            if (user is null || !ValidatePasswordHash(pass, user.Clave))
                return Result.Fail("Usuario o clave incorrectos.", ResultErrorType.Unauthorized);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Nombre),
                new(ClaimTypes.Role, user.Nivel.ToString()),
                new("UserId", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true
            };

            await httpContext.HttpContext!.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            return Result.Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Ocurrió un error al validar el acceso del usuario.");
            return Result.Fail("No fue posible validar el acceso en este momento. Intente de nuevo más tarde.");
        }
    }
}
