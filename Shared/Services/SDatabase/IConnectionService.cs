namespace Coactivos_Predial.Shared.Services.SDatabase
{
    public interface IConnectionService
    {
        string GetConnectionString(string municipio);
        string GetStorageString(string filestorage);
        string GetStorageStringForSector(string? sectorCode);
        string GetStorageStringForMunicipio(string? municipio);
        string GetStorageStringForMunicipio(string? municipio, string? sectorCode);
    }
}
