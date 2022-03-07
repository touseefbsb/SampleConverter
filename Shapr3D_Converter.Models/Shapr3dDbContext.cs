using Microsoft.EntityFrameworkCore;

namespace Shapr3D_Converter.Models
{
    public class Shapr3dDbContext : DbContext
    {
        public DbSet<ModelEntity> ModelEntities { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=Shapr3D.db");
    }
}
