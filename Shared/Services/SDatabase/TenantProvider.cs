using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Shared.AppDbContext;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace Coactivos_Predial.Shared.Services.SDatabase
{
    public interface ITenantProvider
    {
        string? Municipio { get; }
        Task InitializeAsync();
    }

    public class TenantProvider : ITenantProvider
    {
        private readonly AuthenticationStateProvider _auth;
        private readonly IDbContextFactory<MasterDbContext> _masterFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string? Municipio { get; private set; }

        public TenantProvider(AuthenticationStateProvider auth,
                              IDbContextFactory<MasterDbContext> masterFactory,
                              IHttpContextAccessor httpContextAccessor)
        {
            _auth = auth;
            _masterFactory = masterFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InitializeAsync()
        {
            if (Municipio != null) return;

            ClaimsPrincipal? principal = _httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
                principal = (await _auth.GetAuthenticationStateAsync()).User;
            var idStr = principal.FindFirst("UserId")?.Value;
            if (!int.TryParse(idStr, out var id)) return;

            await using var master = await _masterFactory.CreateDbContextAsync();
            Municipio = await master.usuarios
                .Where(u => u.Id == id)
                .Select(u => u.MunicipioNombre)
                .FirstOrDefaultAsync();
        }
    }
}
