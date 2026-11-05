using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.AppDbContext
{
    /// <summary>Contexto del catálogo maestro de usuarios y municipios.</summary>
    public class MasterDbContext : DbContext
    {
        public DbSet<Usuarios> usuarios => Set<Usuarios>();

        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
        {
        }
    }
}
