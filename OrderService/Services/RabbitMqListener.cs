using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OrderService.SharedDto;
using OrderService.Data;
using OrderService.Entities;

namespace OrderService.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "orderQueue";

        public RabbitMqListener(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                try
                {
                    var cartDto = JsonSerializer.Deserialize<CartDto>(message);

                    if (cartDto != null)
                    {
                        await ProcessOrder(cartDto);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task ProcessOrder(CartDto cartDto)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

            var order = new Order
            {
                UserId = cartDto.UserId,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                TotalPrice = cartDto.TotalPrice,
                TotalQuantity = cartDto.TotalQuantity,
                OrderItems = cartDto.Items.Select(item => new OrderItem
                {
                    GameId = item.GameId,
                    GameName = item.GameName,
                    Price = item.Price
                }).ToList()
            };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();

            Console.WriteLine($"✅ Заказ создан: OrderId ={order.Id} UserId={order.UserId}");
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
