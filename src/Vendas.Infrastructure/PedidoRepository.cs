using Microsoft.EntityFrameworkCore;
using Vendas.Domain;

namespace Vendas.Infrastructure
{
    public class PedidoRepository
    {
        private readonly VendasDbContext _context;

        public PedidoRepository(VendasDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Pedido pedido)
        {
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task<Pedido?> GetByIdAsync(Guid id)
        {
            return await _context.Pedidos
                .Include(p => p.Itens) // Inclui os itens do pedido
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}