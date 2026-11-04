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
            return GetStorageStringForMunicipio(_tenantProvider.Municipio);
        }

        public string GetStorageStringForMunicipio(string? municipio)
        {
            var template = _configuration["FileStorageTemplate"] ?? string.Empty;
            if (!string.IsNullOrEmpty(municipio))
            {
                template = template.Replace("{Predial}", municipio).Replace("{Database}", municipio);
            }
            return template;
        }
    }
}
