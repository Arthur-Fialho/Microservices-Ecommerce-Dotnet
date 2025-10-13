
namespace Estoque.Domain
{
    public class Produto
    {
        public Guid Id { get; private set; }
        public string? Nome { get; private set; }
        public string? Descricao { get; private set; }
        public decimal Preco { get; private set; }
        public int QuantidadeEmEstoque { get; private set; }

        // Construtor privado para EF Core
        private Produto() { }

        public Produto(string nome, string descricao, decimal preco, int quantidadeEmEstoque)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do produto não pode ser vazio.", nameof(nome));

            if (preco <= 0)
                throw new ArgumentException("O preço do produto deve ser maior que zero.", nameof(preco));

            if (QuantidadeEmEstoque < 0)
                throw new ArgumentException("A quantidade em estoque não pode ser negativa.", nameof(QuantidadeEmEstoque));

            // Inicialização
            Id = Guid.NewGuid();
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            QuantidadeEmEstoque = quantidadeEmEstoque;
        }
        
        public void DebitarEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("A quantidade a debitar deve ser maior que zero.", nameof(quantidade));

            if (quantidade > QuantidadeEmEstoque)
                throw new InvalidOperationException("Quantidade insuficiente em estoque.");

            QuantidadeEmEstoque -= quantidade;
        }
    }
}