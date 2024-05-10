using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PruebaAtlantidaBE.Models
{
    public class DbPruebaAtlantida : DbContext
    {
        public DbPruebaAtlantida(DbContextOptions<DbPruebaAtlantida> options)
            : base(options)
        {
        }

        public DbSet<TarjetaCreditoCLS> TarjetaCredito { get; set; }
        public DbSet<TipoMovimientoCLS> TipoMovimiento { get; set; }
        public DbSet<MovimientoCLS> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TarjetaCreditoCLS>().HasKey(m => m.Id);
            modelBuilder.Entity<TipoMovimientoCLS>().HasKey(m => m.Id);
            modelBuilder.Entity<MovimientoCLS>().HasKey(m => m.Id);
        }

        public void ActualizarSaldos_Pago(int id, float monto)
        {
            Database.ExecuteSqlRaw("EXEC ActualizarSaldos_Pago @Id, @Monto",
                new SqlParameter("@Id", id),
                new SqlParameter("@Monto", monto));
        }

        public void ActualizarSaldos_Compra(int id, float monto)
        {
            Database.ExecuteSqlRaw("EXEC ActualizarSaldos_Compra @Id, @Monto",
                new SqlParameter("@Id", id),
                new SqlParameter("@Monto", monto));
        }

    }
}
