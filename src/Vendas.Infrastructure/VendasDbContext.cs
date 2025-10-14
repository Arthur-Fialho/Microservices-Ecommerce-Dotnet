using Microsoft.EntityFrameworkCore;
using Vendas.Domain;

namespace Vendas.Infrastructure
{
    public class VendasDbContext : DbContext
    {
        public VendasDbContext(DbContextOptions<VendasDbContext> options) : base(options) { }

        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura o tipo decimal para ValorTotal nos Pedidos
            modelBuilder.Entity<Pedido>()
                .Property(p => p.ValorTotal)
                .HasColumnType("decimal(18,2)");

            // Configura o tipo decimal para PrecoUnitario nos Itens do Pedido
            modelBuilder.Entity<ItemPedido>()
                .Property(p => p.PrecoUnitario)
                .HasColumnType("decimal(18,2)");
        }
    }
}