using System;
using Microsoft.Extensions.Configuration;

namespace Coactivos_Predial.Shared.Services.SDatabase
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantProvider _tenantProvider;

        public ConnectionService(IConfiguration configuration, ITenantProvider tenantProvider)
        {
            _configuration = configuration;
            _tenantProvider = tenantProvider;
        }

        public string GetConnectionString(string municipio)
        {
            var template = _configuration.GetConnectionString("DefaultConnectionTemplate")
                ?? throw new InvalidOperationException("DefaultConnectionTemplate no configurado.");
            return template.Replace("{Database}", municipio);
        }

        public string GetStorageString(string filestorage)
        {
            return GetStorageStringForSector("PREDIAL");
        }

        public string GetStorageStringForSector(string? sectorCode)
        {
            return GetStorageStringForMunicipio(_tenantProvider.Municipio, sectorCode);
        }

        public string GetStorageStringForMunicipio(string? municipio)
        {
            return GetStorageStringForMunicipio(municipio, "PREDIAL");
        }

        public string GetStorageStringForMunicipio(string? municipio, string? sectorCode)
        {
            if (string.IsNullOrWhiteSpace(municipio)
                || municipio is "." or ".."
                || municipio.Contains('/')
                || municipio.Contains('\\'))
            {
                return string.Empty;
            }

            var sectorFolder = sectorCode?.Trim().ToUpperInvariant() switch
            {
                "PREDIAL" => "Predial",
                "INDUSTRIA_COMERCIO" => "IndustriaComercio",
                _ => null
            };
            if (sectorFolder is null) return string.Empty;

            var municipalityFolder = municipio.Trim();
            const string predialDatabasePrefix = "Predial_";
            if (municipalityFolder.StartsWith(predialDatabasePrefix, StringComparison.OrdinalIgnoreCase))
            {
                municipalityFolder = municipalityFolder[predialDatabasePrefix.Length..];
            }
            if (string.IsNullOrWhiteSpace(municipalityFolder)
                || municipalityFolder is "." or ".."
                || municipalityFolder.Contains('/')
                || municipalityFolder.Contains('\\'))
            {
                return string.Empty;
            }

            var template = _configuration["FileStorageTemplate"] ?? string.Empty;
            return template
                .Replace("{Municipio}", municipalityFolder, StringComparison.Ordinal)
                .Replace("{Database}", municipalityFolder, StringComparison.Ordinal)
                .Replace("{Sector}", sectorFolder, StringComparison.Ordinal);
        }
    }
}
