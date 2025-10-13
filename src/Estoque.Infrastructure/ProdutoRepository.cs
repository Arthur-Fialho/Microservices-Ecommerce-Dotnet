using Microsoft.EntityFrameworkCore;
using Estoque.Domain;

namespace Estoque.Infrastructure
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly EstoqueDbContext _context;

        public ProdutoRepository(EstoqueDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Produto produto)
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Produto>> GetAllAsync()
        {
            return await _context.Produtos.ToListAsync();
        }

        public async Task<Produto?> GetByIdAsync(Guid id)
        {
            return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        }

        public void Update(Produto produto)
        {
            _context.Produtos.Update(produto);
            _context.SaveChanges();
        }
    }
}