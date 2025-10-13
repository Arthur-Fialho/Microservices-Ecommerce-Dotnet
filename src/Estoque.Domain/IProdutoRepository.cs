
namespace Estoque.Domain
{
    public interface IProdutoRepository
    {
        Task AddAsync(Produto produto);
        Task<Produto?> GetByIdAsync(Guid id);
        Task<IEnumerable<Produto>> GetAllAsync();
        void Update(Produto produto);

    }
}