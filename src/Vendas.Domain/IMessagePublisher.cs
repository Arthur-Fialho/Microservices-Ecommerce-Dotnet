namespace Vendas.Domain
{
    public interface IMessagePublisher
    {
        void Publish(PedidoCriadoEvent message);
    }
}