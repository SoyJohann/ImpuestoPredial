using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services.SParametros
{
    public class ParametroService : IParametroService
    {
        private readonly ITenantConnectionFactory _connFactory;

        public ParametroService(ITenantConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<int> EditParametros(int id, Parametros parametros)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "UPDATE parametros SET dir = @dir, ciudad = @ciudad, nit = @nit, tel = @tel, vlr_recibo = @vlr_recibo, cuotas_finan=@cuotas_finan,porc_min_fin=@porc_min_fin,fin_nueva_ley=@fin_nueva_ley,cobra_adicional=@cobra_adicional," +
                            "nombre=@nombre,tipo_acuerdo=@tipo_acuerdo,tipo_inicial=@tipo_inicial,divipo=@divipo,fec_ini_interes=@fec_ini_interes,dias_persuasivo=@dias_persuasivo,dh_fecha6=@dh_fecha6 WHERE id = @id";
                return await con.ExecuteAsync(query, parametros);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar los parámetros", ex);
            }
        }

        public async Task<List<Parametros>> GetAll()
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "SELECT * FROM \"Parametros\"";
            return (await con.QueryAsync<Parametros>(query)).ToList();
        }

        public async Task<int> EditEstados(Estados_proc agentes_obj)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "UPDATE estados_proc SET nombre=@nombre,porc=@porc WHERE id = @id";
            return await con.ExecuteAsync(query, agentes_obj);
        }

        public async Task<List<Estados_proc>> Getlist()
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT * FROM \"estados_proc\" limit 10";
                return (await con.QueryAsync<Estados_proc>(query)).ToList();
            }
            catch (NpgsqlException ex)
            {
                throw new Exception("Error al obtener los comparendos", ex);
            }
        }

        public async Task<Parametros> GetParametroById(int id)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT * FROM parametros WHERE id = 1";
                return await con.QueryFirstOrDefaultAsync<Parametros>(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los parámetros por ID: {ex.Message}");
                return null;
            }
        }
    }
}
