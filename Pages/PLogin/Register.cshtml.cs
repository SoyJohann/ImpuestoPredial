using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.SLogin;

namespace Coactivos_Predial.Pages.PLogin
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class RegisterModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoginService _loginService;
        public RegisterModel(IHttpContextAccessor httpContextAccessor,
        ILoginService loginService)
        {
            _httpContextAccessor = httpContextAccessor;
            _loginService = loginService;
        }

        [BindProperty]
        public string email { get; set; }

        [BindProperty]
        public string firstName { get; set; }

        [BindProperty]
        public string password { get; set; }



        public string ErrorMessage { get; set; }
        public bool IsUserRegistrationSuccessfull { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Usuarios usuarios = new();
            usuarios.nombre = firstName;
            usuarios.login = email;
            usuarios.clave = password;
            Console.WriteLine($"paso por el onpost y la clave del usuario es:{email} y el nombre es: {firstName} y la clave es:{password}");
            var registration = await _loginService.UserRegistrationAsync(usuarios);
            if (!registration.Success)
            {
                Console.WriteLine($"paso por registration:{registration}");
            }
            else
            {
                IsUserRegistrationSuccessfull = true;
            }
            return Page();
        }
    }
}
