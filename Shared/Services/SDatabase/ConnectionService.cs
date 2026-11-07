using Coactivos_Predial.Shared.Services.Document.Enums;
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

        public string GetStorageStringForModulo(Modulo tipo) =>
            GetStorageStringForMunicipio(_tenantProvider.Municipio, tipo);

        public string GetStorageStringForTipo(Modulo tipo) =>
            GetStorageStringForMunicipio(_tenantProvider.Municipio, tipo);

        public string GetStorageStringForMunicipio(string? municipio, Modulo tipo)
        {
            var sectorFolder = tipo switch
            {
                Modulo.Predial => "Predial",
                Modulo.IndustriaComercio => "IndustriaComercio",
                _ => null
            };
            if (sectorFolder is null) return string.Empty;

            if (string.IsNullOrWhiteSpace(municipio)
                || municipio is "." or ".."
                || municipio.Contains('/')
                || municipio.Contains('\\'))
            {
                return string.Empty;
            }

            // El nombre de la base municipal debe estar limpio de prefijos
            // técnicos (p. ej. "Cimitarra", no "Predial_Cimitarra"): la misma
            // cadena define la base y la carpeta de almacenamiento.
            var municipalityFolder = municipio.Trim();

            var template = _configuration["FileStorageTemplate"] ?? string.Empty;
            return template
                .Replace("{Municipio}", municipalityFolder, StringComparison.Ordinal)
                .Replace("{Database}", municipalityFolder, StringComparison.Ordinal)
                .Replace("{Sector}", sectorFolder, StringComparison.Ordinal);
        }
    }
}
