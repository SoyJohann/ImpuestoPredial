using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SPropietarios;

/// <summary>Acceso a los datos de propietarios. Solo resuelve la conexión del tenant.</summary>
public partial class PropietariosService(ITenantConnectionFactory connFactory) : IpropietariosService;
