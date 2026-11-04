using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.SDatabase;
using Coactivos_Predial.Shared.Services.SPropietarios;

namespace Coactivos_Predial.Shared.Services.SPropietarios
{
    public class PropietariosService : IpropietariosService
    {
        private readonly ITenantConnectionFactory _connFactory;

        public PropietariosService(ITenantConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        public async Task<int> Add(Propietarios infractores)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "INSERT INTO propietarios(cedula,tipo_ident,nombre,direccion,tel,licencia,ciudad,correo,apellidos,categoria,ciudad_lic) VALUES(@cedula,@tipo_ident,@nombre,@direccion,@tel,@licencia,@ciudad,@correo,@apellidos,@categoria,@ciudad_lic);";
            return await con.ExecuteAsync(query, infractores);
        }

        public int DeleteInfractores(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> EditInfractores(Propietarios infractores)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "UPDATE infractores SET cedula=@cedula,tipo_ident=@tipo_ident,nombre=@nombre,direccion=@direccion,tel=@tel,licencia=@licencia,ciudad=@ciudad,correo=@correo,apellidos=@apellidos,categoria=@categoria,ciudad_lic=@ciudad_lic WHERE id = @id";
            return await con.ExecuteAsync(query, infractores);
        }

        public Propietarios GetInfractores(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Propietarios>> GetList()
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT * FROM \"infractores\" limit 10";
                return (await con.QueryAsync<Propietarios>(query)).ToList();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ocurrió un error al obtener los infractores: " + ex.Message);
                throw new Exception("Error al obtener los comparendos", ex);
            }
        }

        public async Task<string> GetNameInfractorBycode(string code, int tipo)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "SELECT apellidos||' '||nombre as nombre FROM \"infractores\" WHERE cedula = @codigo and tipo_ident= @xtipo";
            return await con.QueryFirstOrDefaultAsync<string>(query, new { codigo = code, xtipo = tipo });
        }

        public async Task<List<Propietarios>> GetAll()
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "SELECT * FROM propietarios limit 10";
                return (await con.QueryAsync<Propietarios>(query)).ToList();
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Ocurrió un error al obtener los propietarios: " + ex.Message);
                throw new Exception("Error al obtener los comparendos", ex);
            }
        }

        public async Task<Propietarios> GetInfractorById(int id)
        {
            try
            {
                await using var con = await _connFactory.CreateOpenAsync();
                var query = "SELECT * FROM propietarios WHERE id = @id";
                var infractor = await con.QueryFirstOrDefaultAsync<Propietarios>(query, new { id });

                if (infractor == null)
                {
                    Console.WriteLine($"No se encontró un infractor con id {id}");
                    return null;
                }

                return infractor;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener infractor por ID", ex);
            }
        }

        public async Task<List<Propietarios>> GetInfractoresByCedula(string cedula)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "SELECT * FROM propietarios WHERE documento like @cedula limit 20";
            return (await con.QueryAsync<Propietarios>(query, new { cedula = cedula + "%" })).ToList();
        }

        public async Task<List<Propietarios>> PropietariosBypredial(string codigo)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "select x.documento,p.tipo_id,x.nombre,t.nombre as ntipo from predios_prop p inner join propietarios x on " +
              " (x.documento=p.cedula and x.tipo_id=p.tipo_id) inner join tipos_ident t on (t.id=x.tipo_id) where p.no_predial = @codigo";
            return (await con.QueryAsync<Propietarios>(query, new { codigo })).ToList();
        }
    }
}
