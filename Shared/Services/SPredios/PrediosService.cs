using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.AppDbContext;
using Coactivos_Predial.Shared.Models;
using Coactivos_Predial.Shared.Services.SDatabase;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Coactivos_Predial.Shared.Services.SPredios
{
    public class PrediosService : IPrediosService
    {
        private readonly ITenantConnectionFactory _connFactory;
        private readonly TenantDbContext _tenantDbContext;

        public PrediosService(ITenantConnectionFactory connFactory, TenantDbContext tenantDbContext)
        {
            _connFactory = connFactory;
            _tenantDbContext = tenantDbContext;
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

        public async Task<Predios> GetPrediosByCode(string comp)
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
        
        public async Task<List<Documentos>> GetPdfsByComp(string comp)
        {
            var codigoPredial = comp.Trim();
            return await _tenantDbContext.Documentos!
                .AsNoTracking()
                .Where(documento => documento.CodigoPredial.Trim() == codigoPredial)
                .OrderBy(documento => documento.Id)
                .ToListAsync();
        }

        public async Task<int> DeleteDocs(int id)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            var query = "DELETE FROM \"Documentos\" WHERE \"Id\" = @id";
            return await con.ExecuteAsync(query, new { id });
        }

        public async Task<int> AgregarDocumento(Documentos documento)
        {
            ArgumentNullException.ThrowIfNull(documento);
            ArgumentException.ThrowIfNullOrWhiteSpace(documento.CodigoPredial);
            ArgumentException.ThrowIfNullOrWhiteSpace(documento.Nombre);
            ArgumentException.ThrowIfNullOrWhiteSpace(documento.Ruta);

            var codigoPredial = documento.CodigoPredial.Trim();
            if (documento.PredioId is null or <= 0)
            {
                throw new InvalidOperationException("Consulte un predio antes de agregarle un documento.");
            }

            if (!string.Equals(Path.GetExtension(documento.Ruta), ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("El documento debe ser un archivo PDF.", nameof(documento));
            }

            var tipoDocumentoId = await _tenantDbContext.TiposDocumento!
                .AsNoTracking()
                .Where(tipo => tipo.Codigo == "POR_CLASIFICAR" && tipo.Sector.Codigo == "PREDIAL")
                .Select(tipo => tipo.Id)
                .SingleOrDefaultAsync();

            if (tipoDocumentoId <= 0)
            {
                throw new InvalidOperationException("No está configurado el tipo documental Predial por clasificar.");
            }

            documento.CodigoPredial = codigoPredial;
            documento.TipoDocumentoId = tipoDocumentoId;
            documento.Fecha = documento.Fecha == default ? DateTime.Today : documento.Fecha;
            documento.Archivo = documento.Ruta;

            await _tenantDbContext.Documentos!.AddAsync(documento);
            return await _tenantDbContext.SaveChangesAsync();
        }

        public async Task<List<DocumentoPredial>> Lista_Diario(DateTime pdesde, DateTime phasta)
        {
            await using var con = await _connFactory.CreateOpenAsync();
            try
            {
                var query = "select num as \"Num\", codigo_predial as \"CodigoPredial\", fecha as \"Fecha\" " +
                            "from pdfs_cargados(@pdesde, @phasta)";
                return (await con.QueryAsync<DocumentoPredial>(query, new { pdesde, phasta })).ToList();
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
