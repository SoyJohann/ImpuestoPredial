using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Coactivos_Predial.Shared.Services.SLogin;


namespace Coactivos_Predial.Pages.PLogin
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class LogInModel : PageModel
    {
        private readonly ILoginService _loginService;
        private readonly IHttpContextAccessor _contextAccessor;

        public LogInModel(IHttpContextAccessor httpContextAccessor, ILoginService loginService)
        {
            _contextAccessor = httpContextAccessor;
            _loginService = loginService;
        }



        [BindProperty]
        public string UserName { get; set; }


        [BindProperty]
        public string pass { get; set; }

        [BindProperty]
        public string mensaje { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            Console.WriteLine($"Usuario: {UserName}, Contrase�a: {pass}");
            var result = await _loginService.UserLoginAsync(UserName, pass);

            if (result == 1)
            {
                mensaje = "Clave Correcta";
                Console.WriteLine("�xito en el inicio de sesi�n");
                return Redirect("/Consultapage");
            }
            else
            {

                mensaje = "Clave Incorrecta";
                Console.WriteLine($"Error en iniciar sesion:{mensaje}");
                TempData["ErrorMessage"] = "Usuario o clave incorrectos. Verifique los datos e intente de nuevo.";
				return Page();
            }
        }


    }
}
