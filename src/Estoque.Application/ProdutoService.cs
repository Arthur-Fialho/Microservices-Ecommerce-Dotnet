using Estoque.Application.DTOs;
using Estoque.Domain;

namespace Estoque.Application
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        // Constructor com abstracao do repositório
        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        // Métodos do serviço
        public async Task<ProdutoDto> CriarProdutoAsync(CriarProdutoDto criarProdutoDto)
        {
            var produto = new Produto
            (
                criarProdutoDto.Nome ?? string.Empty,
                criarProdutoDto.Descricao ?? string.Empty,
                criarProdutoDto.Preco,
                criarProdutoDto.QuantidadeEmEstoque
            );

            await _produtoRepository.AddAsync(produto);

            return new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                QuantidadeEmEstoque = produto.QuantidadeEmEstoque
            };
        }

        public async Task<ProdutoDto?> ObterProdutoPorIdAsync(Guid id)
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null) return null;

            return new ProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                QuantidadeEmEstoque = produto.QuantidadeEmEstoque
            };
        }

        public async Task<IEnumerable<ProdutoDto>> ObterTodosProdutosAsync()
        {
            var produtos = await _produtoRepository.GetAllAsync();
            return produtos.Select(p => new ProdutoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                QuantidadeEmEstoque = p.QuantidadeEmEstoque
            });
        }
    }
}