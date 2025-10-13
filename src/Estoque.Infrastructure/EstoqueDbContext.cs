using Microsoft.EntityFrameworkCore;
using Estoque.Domain;

namespace Estoque.Infrastructure
{
    public class EstoqueDbContext : DbContext
    {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para a entidade Produto
            modelBuilder.Entity<Produto>(entity =>
            {
                // Define que a propriedade 'Preco' deve ser mapeada para uma coluna
                // do tipo decimal(18, 2) no banco de dados.
                // 18 é a precisão total e 2 é o número de casas decimais.
                entity.Property(p => p.Preco).HasColumnType("decimal(18,2)");
            });
        }
    }
}