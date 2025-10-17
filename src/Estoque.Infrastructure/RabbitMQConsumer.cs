using Estoque.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


public class RabbitMQConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "estoque-queue";
    private const string ExchangeName = "pedido-exchange";

    public RabbitMQConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
        _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: "");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var evento = JsonSerializer.Deserialize<PedidoCriadoEvent>(message);

            Console.WriteLine($"--> [ESTOQUE] Mensagem recebida do RabbitMQ: {message}");

            // Processa a baixa de estoque
            await ProcessarBaixaEstoque(evento);
        };

        _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessarBaixaEstoque(PedidoCriadoEvent? evento)
    {
        if (evento == null) return;

        // Um "escopo" para resolver serviços "Scoped", como o DbContext e Repositórios
        using (var scope = _scopeFactory.CreateScope())
        {
            var produtoRepository = scope.ServiceProvider.GetRequiredService<IProdutoRepository>();

            foreach (var item in evento.Itens)
            {
                var produto = await produtoRepository.GetByIdAsync(item.ProdutoId);
                if (produto != null)
                {
                    try
                    {
                        produto.DebitarEstoque(item.Quantidade);
                        produtoRepository.Update(produto);
                        Console.WriteLine($"--> [ESTOQUE] Estoque do produto {produto.Id} debitado em {item.Quantidade}.");
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Lidar com o caso de estoque insuficiente que pode ocorrer
                        // em cenários de alta concorrência. Aqui podemos logar ou notificar.
                        Console.WriteLine($"--> [ESTOQUE] Falha ao debitar estoque para o produto {produto.Id}: {ex.Message}");
                    }
                }
            }
        }
    }
}