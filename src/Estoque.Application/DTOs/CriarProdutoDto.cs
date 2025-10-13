namespace Estoque.Application.DTOs
{
    public class CriarProdutoDto
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeEmEstoque { get; set; }
    }
}