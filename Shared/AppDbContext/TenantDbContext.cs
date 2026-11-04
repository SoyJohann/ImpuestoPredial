using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Models;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.AppDbContext
{
    public class TenantDbContext : DbContext
    {
        public DbSet<Predios>? predios { get; set; }
        public DbSet<Coactivos>? coactivos { get; set; }
        public DbSet<Persuasivos>? persuasivos { get; set; }
        public DbSet<Propietarios>? propietarios { get; set; }
        public DbSet<Parametros>? parametros { get; set; }
        public DbSet<Importados>? importados { get; set; }

        public TenantDbContext(DbContextOptions<TenantDbContext> options) : base(options)
        {
        }
    }
}
