using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.SDatabase;

namespace Coactivos_Predial.Shared.Services
{
    public class PrediosService : IPrediosService
    {
        private readonly ITenantConnectionFactory _connFactory;

        public PrediosService(ITenantConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<List<Predios>> GetAll()
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT c.*,x.apellidos||' '||x.nombre as nombre_inf,a.nombre as nagente FROM comparendos c inner join infractores x on (x.cedula=c.cedula and x.tipo_ident=c.tipo_id) left join agentes a on (a.placa=c.agente) limit 10";
                return (await con.QueryAsync<Predios>(query)).ToList();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ocurrió un error al obtener los comparendos: " + ex.Message);
                throw new Exception("Error al obtener los comparendos", ex);
            }
        }

        public async Task<List<ComparendoInfo>> GetInfoComparendo(string pcedula)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "SELECT codigo,avaluo,dir,matricula,s_nulo(nombre) as nombre FROM maestro WHERE cedula like @cedula";
            var resoluciones = (await con.QueryAsync<ComparendoInfo>(query, new { cedula = pcedula + "%" })).ToList();
            return resoluciones;
        }

        public async Task<Predios> GetMultasByComp(string comp)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT c.*,x.nombre as ntipo FROM maestro c left join tipos_ident x on (x.id=c.tipo) WHERE c.codigo = @comp";
                return await con.QueryFirstOrDefaultAsync<Predios>(query, new { comp });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en consulta Liquidacion: {ex.Message}");
                return new Predios();
            }
        }

        public async Task<List<Predios>> GetComparendosByCedula(string cedula)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "SELECT c.*,s_nulo(x.apellidos)||' '||s_nulo(x.nombre) as nombre_inf FROM comparendos c inner join infractores x on " +
                "(x.cedula=c.cedula and x.tipo_ident=c.tipo_id) where c.cedula like @cedula limit 15";
            return (await con.QueryAsync<Predios>(query, new { cedula = cedula + "%" })).ToList();
        }

        public async Task<List<Documentos>> GetPdfsByComp(string comp)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "select * from muestra_pdfs(@comp)";
            return (await con.QueryAsync<Documentos>(query, new { comp })).ToList();
        }

        public async Task<int> DeleteDocs(int id)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "DELETE FROM documentos WHERE id = @id";
            return await con.ExecuteAsync(query, new { id });
        }

        public async Task<int> Add_Docs(Documentos docs_obj)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "INSERT INTO documentos(codigo_predial,nombre,fecha,archivo) values (@codigo_predial,@nombre,@fecha,@archivo);";
            return await con.ExecuteAsync(query, docs_obj);
        }

        public async Task<List<Documentos>> Lista_Diario(DateTime pdesde, DateTime phasta)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "select * from pdfs_cargados(@pdesde, @phasta)";
                return (await con.QueryAsync<Documentos>(query, new { pdesde, phasta })).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Consulta: {ex.Message}");
                throw;
            }
        }

        public async Task<Item_res> Pdfs_Resumen(DateTime pdesde, DateTime phasta)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "select count(*) as item,sum(num) as num from pdfs_cargados(@pdesde, @phasta)";
                return await con.QueryFirstOrDefaultAsync<Item_res>(query, new { pdesde, phasta });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en consulta Liquidacion: {ex.Message}");
                return new Item_res();
            }
        }

        public async Task<List<Procesos>> GetCoactivo(string comp)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "SELECT p.*,e.nombre as nestado FROM procesos p inner join estados_proc e on (e.id=p.estado) WHERE p.comparendo = @comp";
            return (await con.QueryAsync<Procesos>(query, new { comp })).ToList();
        }
    }
}
