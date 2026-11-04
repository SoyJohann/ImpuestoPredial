using Coactivos_Predial.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Npgsql;
using System.Security.Claims;
using System.Security.Cryptography;
using Dapper;

namespace Coactivos_Predial.Shared.Services.SLogin
{
    public class LoginService : ILoginService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;

        public LoginService(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _configuration = configuration;
            _httpContext = httpContext;
        }

        public async Task<int> UserLoginAsync(string username, string pass)
        {
            // 1. Conectar a la BD MAESTRA para verificar credenciales
            var masterConnStr = _configuration.GetConnectionString("MasterConnection");
            using var masterConn = new NpgsqlConnection(masterConnStr);
            await masterConn.OpenAsync();

            // 2. Buscar usuario en la BD maestra
            var user = await masterConn.QueryFirstOrDefaultAsync<UsuarioLogin>(
                @"SELECT id, login, clave, mun, nombre, nivel 
                  FROM usuarios WHERE login = @user", new { user = username.ToLower() });

            if (user == null)
            {
                return 0; // Falso
            }

            // 3. Validar hash (SEGURIDAD: No se compara texto plano)
            if (!ValidatePasswordHash(pass, user.clave))
            {
                return 0; // Falso
            }

            // Guardar claims (solo Name, Role y UserId según especificación multi-tenant)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.nombre),
                new Claim(ClaimTypes.Role, user.nivel.ToString()),
                new Claim("UserId", user.id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true
            };

            await _httpContext.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return 1; // Verdadero
        }

        // Método requerido por la interfaz ILoginService
        public async Task<(bool Success, string Message)> UserRegistrationAsync(Usuarios usuarios)
        {
            // Implementación stub: en este módulo solo se manejan logins.
            // Se retorna falso para indicar que el registro no está soportado en este servicio.
            return (false, "El registro de nuevos usuarios no está soportado en este servicio. Contacte al administrador.");
        }

        // Método público para validar hash (necesario por la interfaz ILoginService)
        public bool ValidatePasswordHash(string password, string storedHash)
        {
            byte[] dbHashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Array.Copy(dbHashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hashBytes = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (dbHashBytes[i + 16] != hashBytes[i])
                    return false;
            }
            return true;
        }
    }

    // Modelo simple para el query a la BD maestra
    public class UsuarioLogin
    {
        public int id { get; set; }
        public string login { get; set; }
        public string clave { get; set; }
        public string mun { get; set; }
        public string nombre { get; set; }
        public int nivel { get; set; }
    }
}