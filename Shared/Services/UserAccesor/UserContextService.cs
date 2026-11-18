using System.Security.Claims;
using Coactivos_Predial.Shared.Services.SDatabase;
using Microsoft.AspNetCore.Components.Authorization;

namespace Coactivos_Predial.Shared.Services.UserAccesor
{
    public class UserContextService : IUserContextService
    {
        private readonly ITenantProvider _tenantProvider;
        private readonly AuthenticationStateProvider _auth;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal? _principal;

        public UserContextService(
            ITenantProvider tenantProvider,
            AuthenticationStateProvider auth,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantProvider = tenantProvider;
            _auth = auth;
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserMunicipio()
        {
            return _tenantProvider.Municipio;
        }

        public async Task<int> GetUserIdAsync()
        {
            var principal = await GetPrincipalAsync();
            return int.TryParse(principal.FindFirst("UserId")?.Value, out var id) ? id : 0;
        }

        public async Task<string?> GetUserNameAsync()
        {
            var principal = await GetPrincipalAsync();
            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }

        public async Task<bool> IsInRoleAsync(string role)
        {
            var principal = await GetPrincipalAsync();
            return principal.Identity?.IsAuthenticated == true
                && (principal.IsInRole(role)
                    || principal.Claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == role));
        }

        private async Task<ClaimsPrincipal> GetPrincipalAsync()
        {
            if (_principal is not null)
            {
                return _principal;
            }

            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                principal = (await _auth.GetAuthenticationStateAsync()).User;
            }

            _principal = principal;
            return principal;
        }
    }
}
