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
    }
}