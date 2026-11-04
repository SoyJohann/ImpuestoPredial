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

namespace Coactivos_Predial.Shared.Services.SCoactivos
{
    public class CoactivosService : ICoactivosService
    {
        private readonly ITenantConnectionFactory _connFactory;

        public CoactivosService(ITenantConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<List<Coactivos>> GetList(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT * FROM sin_Coactivo('" + opcion + "', '" + pcomp + "','" + pdesde + "','" + phasta + "','" + pvigencia + "','" + ptran + "')";
                return (await con.QueryAsync<Coactivos>(query)).ToList();
            }
            catch (Exception)
            {
                throw new Exception("no se encontraron coactivos");
            }
        }

        public async Task<List<Coactivos>> Lista_Coactivos(int opcion, string pcomp, int pvigencia, int ptran, int ptipo)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "select * from reporte_coactivos(" + opcion + ",'" + pcomp + "'," + pvigencia + "," + ptran + "," + ptipo + ") order by id";
                Console.WriteLine($"sarta : {query}");
                return (await con.QueryAsync<Coactivos>(query)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Sin Registros Coactivos: " + ex.Message);
            }
        }

        public async Task<List<Coactivos>> GetList1(string x)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "SELECT * FROM sin_coactivo()";
            return (await con.QueryAsync<Coactivos>(query)).ToList();
        }

        public async Task<int> Procesa_Coactivo(int opcion, string pcomp, DateTime pdesde, DateTime phasta, int pvigencia, int ptran)
        {
            try
            {
                await using var connection = await _connFactory.CreateOpenAsync();
                await using var cmd = new NpgsqlCommand();
                cmd.CommandText = "SELECT public.mandamiento(@opcion,@pcomp,@pdesde,@phasta,@pvigencia,@ptran)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.Add(new NpgsqlParameter("@opcion", NpgsqlDbType.Integer) { Value = opcion });
                cmd.Parameters.Add(new NpgsqlParameter("@pcomp", NpgsqlDbType.Text) { Value = pcomp });
                cmd.Parameters.Add(new NpgsqlParameter("@pdesde", NpgsqlDbType.Date) { Value = pdesde });
                cmd.Parameters.Add(new NpgsqlParameter("@phasta", NpgsqlDbType.Date) { Value = phasta });
                cmd.Parameters.Add(new NpgsqlParameter("@pvigencia", NpgsqlDbType.Integer) { Value = pvigencia });
                cmd.Parameters.Add(new NpgsqlParameter("@ptran", NpgsqlDbType.Integer) { Value = ptran });
                Console.WriteLine("Ejecutando Mandamiento");
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Mandamiento: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<Transac_Report>> GetListtran(string ptipo, int pclase)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                string query;
                if (pclase == 1)
                {
                    query = "select p.num_proc as transaccion,cast(p.fecha_proc as date) as fecha,count(*) as No from procesos p " +
                    "inner join comparendos c on (c.comparendo=p.comparendo) where p.tipo='" + ptipo + "' and c.estado=1 group by p.num_proc,p.fecha_proc ";
                }
                else
                {
                    query = "select p.num_proc as transaccion,cast(p.fecha_proc as date) as fecha,count(*) as No from procesos p " +
                    "inner join comparendos c on (c.comparendo=p.comparendo) where p.tipo='" + ptipo + "' group by p.num_proc,p.fecha_proc ";
                }
                return (await con.QueryAsync<Transac_Report>(query)).ToList();
            }
            catch (Exception)
            {
                throw new Exception("no se encontraron coactivos");
            }
        }
    }
}
