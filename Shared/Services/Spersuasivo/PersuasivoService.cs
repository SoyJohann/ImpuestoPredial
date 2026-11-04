using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.Spersuasivo
{
    public class PersuasivoService : IPersuasivoService
    {
        private readonly ITenantConnectionFactory _connFactory;

        public PersuasivoService(ITenantConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<List<Persuasivos>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT * FROM sin_persuasivo('" + opcion + "', '" + pcomp + "','" + pdesde + "','" + phasta + "','" + pvigencia + "','" + ptran + "')";
                return (await con.QueryAsync<Persuasivos>(query)).ToList();
            }
            catch (Exception)
            {
                throw new Exception("no se encontraron coactivos");
            }
        }

        public async Task<int> Procesa_Per(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran)
        {
            try
            {
                await using var connection = await _connFactory.CreateOpenAsync();
                await using var cmd = new NpgsqlCommand();
                cmd.CommandText = "SELECT public.persuasivo(@opcion,@pcomp,@pdesde,@phasta,@pvigencia,@ptran)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.Add(new NpgsqlParameter("@opcion", NpgsqlDbType.Integer) { Value = opcion });
                cmd.Parameters.Add(new NpgsqlParameter("@pcomp", NpgsqlDbType.Text) { Value = pcomp });
                cmd.Parameters.Add(new NpgsqlParameter("@pdesde", NpgsqlDbType.Date) { Value = pdesde });
                cmd.Parameters.Add(new NpgsqlParameter("@phasta", NpgsqlDbType.Date) { Value = phasta });
                cmd.Parameters.Add(new NpgsqlParameter("@pvigencia", NpgsqlDbType.Integer) { Value = pvigencia });
                cmd.Parameters.Add(new NpgsqlParameter("@ptran", NpgsqlDbType.Integer) { Value = ptran });
                Console.WriteLine("Ejecutando persuasivos");
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Persuasivo: {ex.Message}");
                return 0;
            }
        }
    }
}
