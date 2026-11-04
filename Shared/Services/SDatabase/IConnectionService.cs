namespace Coactivos_Predial.Shared.Services.SDatabase
{
    public interface IConnectionService
    {
        string GetConnectionString(string municipio);
        string GetStorageString(string filestorage);
        string GetStorageStringForMunicipio(string? municipio);
    }
}
