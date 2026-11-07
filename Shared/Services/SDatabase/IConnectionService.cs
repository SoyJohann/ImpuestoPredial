using Coactivos_Predial.Shared.Services.Document.Enums;

namespace Coactivos_Predial.Shared.Services.SDatabase;

public interface IConnectionService
{
    string GetConnectionString(string municipio);
    string GetStorageStringForModulo(Modulo tipo);
    string GetStorageStringForTipo(Modulo tipo);
    string GetStorageStringForMunicipio(string? municipio, Modulo tipo);
}
