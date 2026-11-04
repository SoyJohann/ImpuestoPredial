using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Coactivos_Predial.Shared.Services.SDatabase
{
    public interface ITenantConnectionFactory
    {
        Task<NpgsqlConnection> CreateOpenAsync();
        string ConnectionString { get; }
    }

    public class TenantConnectionFactory : ITenantConnectionFactory
    {
        private readonly ITenantProvider _tenant;
        private readonly IConfiguration _config;

        public TenantConnectionFactory(ITenantProvider tenant, IConfiguration config)
        {
            _tenant = tenant;
            _config = config;
        }

        public string ConnectionString
        {
            get
            {
                var mun = _tenant.Municipio
                    ?? throw new InvalidOperationException("Tenant no inicializado");
                return _config.GetConnectionString("DefaultConnectionTemplate")!
                              .Replace("{Database}", mun);
            }
        }

        public async Task<NpgsqlConnection> CreateOpenAsync()
        {
            var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
}
