using Coactivos_Predial.Shared.AppDbContext;

namespace Coactivos_Predial.Shared.Services.SContribuyentes;

/// <summary>
/// Acceso a los datos de industria y comercio. Cada operación vive en su propio archivo parcial.
/// </summary>
public partial class ContribuyentesService(TenantDbContext tenantDbContext) : IContribuyentesService
{
}
