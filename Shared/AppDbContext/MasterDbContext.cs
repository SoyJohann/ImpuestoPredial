using Microsoft.EntityFrameworkCore;
using Coactivos_Predial.Shared.Models;

namespace Coactivos_Predial.Shared.AppDbContext
{
    public class MasterDbContext : DbContext
    {
        public DbSet<Usuarios> usuarios { get; set; } = null!;

        public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
        {
        }
    }
}
