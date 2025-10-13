using Vendas.Domain.DTOs;

namespace Vendas.Domain
{
    public interface IEstoqueServiceHttpClient
    {
        Task<ProdutoDto?> GetProdutoByIdAsync(Guid produtoId);
    }
}