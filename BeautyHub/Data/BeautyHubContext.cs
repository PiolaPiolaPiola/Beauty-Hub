using BeautyHub.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautyHub.Data
{
    public class BeautyHubContext : DbContext
    {
        public BeautyHubContext(DbContextOptions<BeautyHubContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Profesional> Profesionales => Set<Profesional>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Servicio> Servicios => Set<Servicio>();
        public DbSet<Cita> Citas => Set<Cita>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Servicio>()
                .Property(s => s.Categoria)
                .HasConversion<string>();

            modelBuilder.Entity<Cita>()
                .Property(c => c.Estado)
                .HasConversion<string>();
        }
    }
}
