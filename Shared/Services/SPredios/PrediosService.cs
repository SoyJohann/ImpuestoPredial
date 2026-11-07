using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Services.SDatabase;
using Coactivos_Predial.Shared.Services.UserAccesor;

namespace Coactivos_Predial.Shared.Services.SPredios;

/// <summary>
/// Acceso a los datos del predio. La conexión Dapper del tenant, el contexto EF
/// y el usuario autenticado se resuelven aquí; cada operación vive en su propio archivo parcial.
/// </summary>
public partial class PrediosService (
    ITenantConnectionFactory connFactory,
    TenantDbContext tenantDbContext,
    IUserContextService userContext) : IPrediosService
{
}
