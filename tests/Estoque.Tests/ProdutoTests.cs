using Xunit;
using Estoque.Domain;

public class ProdutoTests
{
    [Fact]
    public void DebitarEstoque_ComEstoqueSuficiente_DeveAtualizarQuantidadeCorretamente()
    {
        // Arrange (Organizar)
        var produto = new Produto("Teste", "Desc", 10, 20); // 20 em estoque
        int quantidadeDebito = 5;
        int estoqueEsperado = 15;

        // Act (Agir)
        produto.DebitarEstoque(quantidadeDebito);

        // Assert (Verificar)
        Assert.Equal(estoqueEsperado, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public void DebitarEstoque_ComEstoqueInsuficiente_DeveLancarInvalidOperationException()
    {
        // Arrange (Organizar)
        var produto = new Produto("Teste", "Desc", 10, 5); // 5 em estoque
        int quantidadeDebito = 10;

        // Act (Agir) & Assert (Verificar)
        // Verifica se a ação de debitar lança a exceção esperada
        Assert.Throws<InvalidOperationException>(() => produto.DebitarEstoque(quantidadeDebito));
    }

    [Fact]
    public void Construtor_ComPrecoZero_DeveLancarArgumentException()
    {
        // Arrange (Organizar)
        string nome = "Teste";
        string desc = "Desc";
        decimal precoInvalido = 0;
        int estoque = 10;

        // Act (Agir) & Assert (Verificar)
        // Verifica se a ação de criar o produto lança a exceção esperada
        Assert.Throws<ArgumentException>(() => new Produto(nome, desc, precoInvalido, estoque));
    }
}