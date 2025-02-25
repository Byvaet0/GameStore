using CartService.Data;
using CartService.DTOs;
using CartService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CartService.Services
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private const string QueueName = "cartQueue";

        public RabbitMqListener(IServiceScopeFactory serviceScopeFactory)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _serviceScopeFactory = serviceScopeFactory;

            _channel.QueueDeclare(
            queue: QueueName,
            durable: true,  // Параметр durability для долговечности очереди
            exclusive: false,
            autoDelete: false,
            arguments: null
);

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
                    var cartMessage = JsonSerializer.Deserialize<CartMessage>(message);
                    Console.WriteLine($"[x] Получено сообщение: UserId={cartMessage.UserId}, Game={cartMessage.GameName}, Price={cartMessage.Price}");

                    // Используем IServiceScope для создания скоупа
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<CartDbContext>();

                        // Обрабатываем полученные данные и сохраняем в БД
                        var cart = await dbContext.Carts
                            .Include(c => c.Items)
                            .FirstOrDefaultAsync(c => c.UserId == cartMessage.UserId);

                        if (cart == null)
                        {
                            cart = new Cart
                            {
                                UserId = cartMessage.UserId,
                                TotalQuantity = 1,
                                TotalPrice = cartMessage.Price,
                            };

                              var newItem = new CartItem
                             {
                                GameId = cartMessage.GameId,
                                GameName = cartMessage.GameName,
                                Price = cartMessage.Price
                            };

    cart.Items.Add(newItem); // Добавляем товар в корзину

                            dbContext.Carts.Add(cart);
                        }
                        else
                        {
                            // Обновляем корзину
                             var existingItem = cart.Items.FirstOrDefault(ci => ci.GameId == cartMessage.GameId);

    if (existingItem == null)
    {
        var newItem = new CartItem
        {
            CartId = cart.CartId, // Устанавливаем CartId явно
            GameId = cartMessage.GameId,
            GameName = cartMessage.GameName,
            Price = cartMessage.Price
        };

        dbContext.CartItems.Add(newItem); // Добавляем товар в БД
    }

    cart.TotalQuantity = cart.Items.Count;;
    cart.TotalPrice += cartMessage.Price;
}

                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
