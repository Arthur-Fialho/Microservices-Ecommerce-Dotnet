namespace Vendas.Application.DTOs
{
    public class CriarPedidoItemDto
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}