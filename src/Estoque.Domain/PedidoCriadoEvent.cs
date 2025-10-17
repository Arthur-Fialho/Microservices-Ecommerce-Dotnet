// DTO para o item
public class PedidoCriadoItem
{
    public Guid ProdutoId { get; set; }
    public int Quantidade { get; set; }
}

// O evento que será recebido
public class PedidoCriadoEvent
{
    public Guid PedidoId { get; set; }
    public List<PedidoCriadoItem> Itens { get; set; }
}