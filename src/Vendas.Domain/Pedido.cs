
namespace Vendas.Domain
{
    public class Pedido
    {
    public Guid Id { get; private set; }
    public DateTime DataPedido { get; private set; }
    public decimal ValorTotal { get; private set; }

    // Encapsulamento da coleção de itens
    private readonly List<ItemPedido> _itens = new List<ItemPedido>();
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    public Pedido()
    {
        Id = Guid.NewGuid();
        DataPedido = DateTime.UtcNow;
        ValorTotal = 0;
    }

    public void AdicionarItem(ItemPedido item)
    {
        _itens.Add(item);
        CalcularValorTotal();
    }

    private void CalcularValorTotal()
    {
        ValorTotal = _itens.Sum(item => item.Quantidade * item.PrecoUnitario);
    }
    }
}