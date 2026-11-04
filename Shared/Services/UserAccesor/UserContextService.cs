using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.UserAccesor
{
    public class UserContextService : IUserContextService
    {
        private readonly ITenantProvider _tenantProvider;

        public UserContextService(ITenantProvider tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        public string? GetUserMunicipio()
        {
            return _tenantProvider.Municipio;
        }
    }
}
