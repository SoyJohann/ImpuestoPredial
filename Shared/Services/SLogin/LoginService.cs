using Coactivos_Predial.Shared.AppDbContext;

namespace Coactivos_Predial.Shared.Services.SLogin;

/// <summary>Autenticación contra el catálogo maestro de usuarios.</summary>
public partial class LoginService : ILoginService
{
    private readonly IHttpContextAccessor httpContext;
    private readonly ILogger<LoginService> logger;
    private readonly MasterDbContext masterContext;

    public LoginService(IHttpContextAccessor httpContext, ILogger<LoginService> logger, MasterDbContext masterContext)
    {
        this.httpContext = httpContext;
        this.logger = logger;
        this.masterContext = masterContext;
    }
}
