using Vendas.Application.DTOs;
using Vendas.Domain;

namespace Vendas.Application
{
    public class PedidoService : IPedidoService
    {
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IEstoqueServiceHttpClient _estoqueService;
    private readonly IMessagePublisher _messagePublisher;

    public PedidoService(IPedidoRepository pedidoRepository, IEstoqueServiceHttpClient estoqueService, IMessagePublisher messagePublisher)
    {
        _pedidoRepository = pedidoRepository;
        _estoqueService = estoqueService;
        _messagePublisher = messagePublisher;
    }

    public async Task<bool> CriarPedidoAsync(CriarPedidoDto criarPedidoDto)
    {
        var novoPedido = new Pedido();

        foreach (var itemDto in criarPedidoDto.Itens)
        {
            // 1. VALIDAÇÃO: Chama o microserviço de Estoque
            var produtoDto = await _estoqueService.GetProdutoByIdAsync(itemDto.ProdutoId);
            
            // Verifica se o produto é nulo ANTES de tentar usar suas propriedades.
            if (produtoDto == null || produtoDto.QuantidadeEmEstoque < itemDto.Quantidade)
            {
                return false; // Produto não existe ou estoque insuficiente
            }

            // 2. CRIAÇÃO: Adiciona o item ao pedido
            var itemPedido = new ItemPedido(produtoDto.Id, itemDto.Quantidade, produtoDto.Preco);
            novoPedido.AdicionarItem(itemPedido);
        }

        // 3. PERSISTÊNCIA: Salva o novo pedido no banco de dados de Vendas
        await _pedidoRepository.AddAsync(novoPedido);

        // 4. PUBLICAÇÃO: Notifica outros serviços que um pedido foi criado.
        var evento = new PedidoCriadoEvent
        {
            PedidoId = novoPedido.Id,
            Itens = novoPedido.Itens.Select(item => new PedidoCriadoItem
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade
            }).ToList()
        };
        _messagePublisher.Publish(evento); // Dispara a mensagem e não espera por resposta!

        
        return true;
    }

    }
}