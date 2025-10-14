using Vendas.Application.DTOs;
using Vendas.Domain;

namespace Vendas.Application
{
    public class PedidoService : IPedidoService
    {
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IEstoqueServiceHttpClient _estoqueService;

    public PedidoService(IPedidoRepository pedidoRepository, IEstoqueServiceHttpClient estoqueService)
    {
        _pedidoRepository = pedidoRepository;
        _estoqueService = estoqueService;
    }

    public async Task<bool> CriarPedidoAsync(CriarPedidoDto criarPedidoDto)
    {
        var novoPedido = new Pedido();

        foreach (var itemDto in criarPedidoDto.Itens)
        {
            // 1. VALIDAÇÃO: Chama o microserviço de Estoque
            var produtoDto = await _estoqueService.GetProdutoByIdAsync(itemDto.ProdutoId);

            // Regras de negócio
            if (produtoDto == null) return false; // Produto não existe
            if (produtoDto.QuantidadeEmEstoque < itemDto.Quantidade) return false; // Estoque insuficiente

            // 2. CRIAÇÃO: Adiciona o item ao pedido
            var itemPedido = new ItemPedido(produtoDto.Id, itemDto.Quantidade, produtoDto.Preco);
            novoPedido.AdicionarItem(itemPedido);
        }

        // 3. PERSISTÊNCIA: Salva o novo pedido no banco de dados de Vendas
        await _pedidoRepository.AddAsync(novoPedido);

        // TODO: Notificar o serviço de estoque para dar baixa.

        return true;
    }

    }
}