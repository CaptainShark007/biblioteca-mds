using Microsoft.EntityFrameworkCore;
using trabajoMetodologiaDSistemas.Models;

namespace trabajoMetodologiaDSistemas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Socio> Socios { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<Multa> Multas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Socio>()
                .HasIndex(s => s.Dni)
                .IsUnique();

            modelBuilder.Entity<Libro>()
                .HasIndex(l => l.ISBN)
                .IsUnique();

        }
    }
}
