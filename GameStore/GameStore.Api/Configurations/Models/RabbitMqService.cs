using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

public class RabbitMqService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string QueueName = "cartQueue";

    public RabbitMqService()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

       
    }

    public void PublishMessage<T>(T message)  // Сериализуем объект (например, AddToCartDto) в JSON и отправляем его в очередь
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange: "",
                              routingKey: QueueName,
                              basicProperties: null,
                              body: body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
