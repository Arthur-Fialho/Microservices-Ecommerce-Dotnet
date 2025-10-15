using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Vendas.Domain;

public class RabbitMQPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private const string ExchangeName = "pedido-exchange";

    public RabbitMQPublisher()
    {
        // Configura a conexão com o RabbitMQ.
        try
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Falha ao conectar no RabbitMQ: {ex.Message}");
            throw;
        }
    }

    public void Publish(PedidoCriadoEvent message)
    {
        // Cria um canal para comunicação com o RabbitMQ.
        using (var channel = _connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: ExchangeName,
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            
            Console.WriteLine($"--> [VENDAS] Mensagem publicada no RabbitMQ: {json}");
        }
    }

    public void Dispose()
    {
        // Fecha a conexão quando a aplicação for encerrada.
        _connection?.Dispose();
    }
}