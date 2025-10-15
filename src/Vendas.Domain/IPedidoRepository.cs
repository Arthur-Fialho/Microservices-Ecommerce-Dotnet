namespace Vendas.Domain
{
    public interface IPedidoRepository
    {
        Task AddAsync(Pedido pedido);
        Task<Pedido?> GetByIdAsync(Guid id);
    }
}