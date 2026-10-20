using Microsoft.EntityFrameworkCore;
using GestorDeMercados.Models;

namespace GestorDeMercados.Data
{
    // La clase APPDbContext actúa como el puente principal entre el código C# y la base de datos SQL Server
    public class APPDbContext : DbContext
    {
        public APPDbContext(DbContextOptions<APPDbContext> options) : base(options)
        {
        }

        // Definición de las tablas (DbSets) que representarán las entidades del Mercadito
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Dueno> Duenos { get; set; }
        public DbSet<Tienda> Tiendas { get; set; }
        public DbSet<Deuda> Deudas { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        // Fluent API: Aquí se configuran las reglas de integridad y formato que no se pueden definir solo con clases
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relación 1 a 1 entre Pago y Deuda:
            // Una deuda solo puede tener un pago único y un pago pertenece a una sola deuda.
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Deuda)
                .WithOne(d => d.Pago)
                .HasForeignKey<Pago>(p => p.DeudaId)
                .OnDelete(DeleteBehavior.Restrict); // Evita que se borre una deuda si ya tiene un pago registrado

            // Garantiza que no se dupliquen pagos para una misma deuda (ID único)
            modelBuilder.Entity<Pago>()
                .HasIndex(p => p.DeudaId)
                .IsUnique();

            // Configuración de precisión decimal para campos monetarios:
            // Esto asegura que en SQL Server se guarden como DECIMAL(18,2) para evitar errores de redondeo en soles.
            modelBuilder.Entity<Pago>()
                .Property(p => p.MontoPago)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Deuda>()
                .Property(d => d.MontoDeuda)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Servicio>()
                .Property(s => s.Costo)
                .HasPrecision(18, 2);
        }
    }
}