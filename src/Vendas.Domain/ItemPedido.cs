namespace Vendas.Domain
{
    public class ItemPedido
    {
        public Guid Id { get; private set; }
        public Guid ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal PrecoUnitario { get; private set; }

        // Construtor privado para EF Core
        private ItemPedido() { }

        public ItemPedido(Guid produtoId, int quantidade, decimal precoUnitario)
        {
            if(produtoId == Guid.Empty)
                throw new ArgumentException("O ID do produto não pode ser vazio.", nameof(produtoId));
            if(quantidade <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));
            if(precoUnitario <= 0)
                throw new ArgumentException("O preço unitário deve ser maior que zero.", nameof(precoUnitario));

            Id = Guid.NewGuid();
            ProdutoId = produtoId;
            Quantidade = quantidade;
            PrecoUnitario = precoUnitario;
        }
    }
}