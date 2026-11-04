using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SImportados
{
    public class ImportadosService : IimportadosService
    {
        private readonly ITenantConnectionFactory _connFactory;

        public ImportadosService(ITenantConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<List<Importados>> GetList()
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT * FROM \"importados\" limit 20";
                return (await con.QueryAsync<Importados>(query)).ToList();
            }
            catch (Exception)
            {
                throw new Exception("no se encontraron Registros");
            }
        }

        public async Task<int> DeleteAll()
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "delete from importados;";
            await con.ExecuteAsync(query);
            return 0;
        }

        public async Task<int> DeleteRec()
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "delete from simit_rec;";
            await con.ExecuteAsync(query);
            return 0;
        }

        public async Task<int> Procesa_Rec(int puser)
        {
            try
            {
                await using var connection = await _connFactory.CreateOpenAsync();
                await using var cmd = new NpgsqlCommand();
                cmd.CommandText = "SELECT public.procesa_rec(@puser)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add(new NpgsqlParameter("@puser", NpgsqlDbType.Integer) { Value = puser });
                Console.WriteLine("Ejecutando Crea_incumplidos");
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GenerarAcuerdo: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> Procesa_Comp(int puser)
        {
            try
            {
                await using var connection = await _connFactory.CreateOpenAsync();
                await using var cmd = new NpgsqlCommand();
                cmd.CommandText = "SELECT public.procesa_import(@puser)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                cmd.Parameters.Add(new NpgsqlParameter("@puser", NpgsqlDbType.Integer) { Value = puser });
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Procesar Importados : {ex.Message}");
                return 0;
            }
        }

        public async Task<int> Add(Importados import_obj)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "INSERT INTO importados (codigo,cedula,nombre,dir,area_tet,area_con,avaluo,tipo) " +
                  "VALUES (@codigo,@cedula,@nombre,@dir,@area_tet,@area_con,@avaluo,@tipo)";
                return await con.ExecuteAsync(query, import_obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando importados: {ex.Message}");
                throw;
            }
        }
    }
}
