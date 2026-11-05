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
            var result = await _loginService.UserLoginAsync(UserName, pass);

            if (result.IsSuccess)
            {
                mensaje = "Clave Correcta";
                return Redirect("/Consultapage");
            }
            else
            {

                mensaje = "Clave Incorrecta";
                TempData["ErrorMessage"] = result.Type == Coactivos_Predial.Shared.Services.Common.ResultErrorType.Unauthorized
                    ? "Usuario o clave incorrectos. Verifique los datos e intente de nuevo."
                    : result.Error ?? "No fue posible validar el acceso en este momento.";
				return Page();
            }
        }


    }
}
