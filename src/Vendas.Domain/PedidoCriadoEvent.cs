namespace Vendas.Domain
{
    // DTO para os itens do pedido
    public class PedidoCriadoItem
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
    public class PedidoCriadoEvent
    {
        // O evento principal
        public Guid PedidoId { get; set; }
        public List<PedidoCriadoItem>? Itens { get; set; }
    }
}