using Moq;
using Vendas.Application;
using Vendas.Application.DTOs;
using Vendas.Domain;
using Vendas.Domain.DTOs;

public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IEstoqueServiceHttpClient> _estoqueServiceMock;
    private readonly PedidoService _pedidoService;

    // O construtor é executado antes de cada teste, 
    // preparando um ambiente limpo com mocks novos.
    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _estoqueServiceMock = new Mock<IEstoqueServiceHttpClient>();
        
        // Injeta os MOCKS (objetos falsos) no serviço real
        _pedidoService = new PedidoService(
            _pedidoRepositoryMock.Object, 
            _estoqueServiceMock.Object,
            new Mock<IMessagePublisher>().Object
        );
    }

    [Fact]
    public async Task CriarPedidoAsync_ComEstoqueSuficiente_DeveRetornarTrueESalvarPedido()
    {
        // Arrange (Organizar)
        var produtoId = Guid.NewGuid();
        var dto = new CriarPedidoDto
        {
            Itens = new List<CriarPedidoItemDto> { new CriarPedidoItemDto { ProdutoId = produtoId, Quantidade = 2 } }
        };

        var produtoEstoqueDto = new ProdutoDto { Id = produtoId, Preco = 10, QuantidadeEmEstoque = 5 };

        // Mock de Estoque: "Quando o método GetProdutoByIdAsync for chamado COM ESTE produtoId,
        // então retorne o nosso objeto produtoEstoqueDto."
        _estoqueServiceMock.Setup(s => s.GetProdutoByIdAsync(produtoId))
            .ReturnsAsync(produtoEstoqueDto);

        // Act (Agir)
        var resultado = await _pedidoService.CriarPedidoAsync(dto);

        // Assert (Verificar)
        Assert.True(resultado); // O serviço deve retornar sucesso

        // Verifica se o método AddAsync do repositório foi chamado EXATAMENTE UMA VEZ.
        // Isso prova que tentamos salvar o pedido.
        _pedidoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Pedido>()), Times.Once);
    }

    [Fact]
    public async Task CriarPedidoAsync_ComEstoqueInsuficiente_DeveRetornarFalse()
    {
        // Arrange (Organizar)
        var produtoId = Guid.NewGuid();
        var dto = new CriarPedidoDto
        {
            Itens = new List<CriarPedidoItemDto> { new CriarPedidoItemDto { ProdutoId = produtoId, Quantidade = 10 } }
        };

        var produtoEstoqueDto = new ProdutoDto { Id = produtoId, Preco = 10, QuantidadeEmEstoque = 5 }; // Só 5 em estoque

        // Configura o Mock de Estoque
        _estoqueServiceMock.Setup(s => s.GetProdutoByIdAsync(produtoId))
            .ReturnsAsync(produtoEstoqueDto);

        // Act (Agir)
        var resultado = await _pedidoService.CriarPedidoAsync(dto);

        // Assert (Verificar)
        Assert.False(resultado); // O serviço deve retornar falha

        // Verifica se o método AddAsync NUNCA foi chamado.
        // Isso prova que a lógica de validação parou a execução antes de salvar.
        _pedidoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Pedido>()), Times.Never);
    }
}