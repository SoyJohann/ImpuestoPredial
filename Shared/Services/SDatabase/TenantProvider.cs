using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Shared.AppDbContext;
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

        public string? Municipio { get; private set; }

        public TenantProvider(AuthenticationStateProvider auth,
                              IDbContextFactory<MasterDbContext> masterFactory)
        {
            _auth = auth;
            _masterFactory = masterFactory;
        }

        public async Task InitializeAsync()
        {
            if (Municipio != null) return;

            var state = await _auth.GetAuthenticationStateAsync();
            var idStr = state.User.FindFirst("UserId")?.Value;
            if (!int.TryParse(idStr, out var id)) return;

            await using var master = await _masterFactory.CreateDbContextAsync();
            Municipio = await master.usuarios
                .Where(u => u.id == id)
                .Select(u => u.mun)
                .FirstOrDefaultAsync();
        }
    }
}
