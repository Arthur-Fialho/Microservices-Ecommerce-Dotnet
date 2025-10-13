using Estoque.Application.DTOs;

namespace Estoque.Application
{
    public interface IProdutoService
    {
        Task<ProdutoDto> CriarProdutoAsync(CriarProdutoDto criarProdutoDto);
        Task<ProdutoDto?> ObterProdutoPorIdAsync(Guid id);
        Task<IEnumerable<ProdutoDto>> ObterTodosProdutosAsync();
    }
}