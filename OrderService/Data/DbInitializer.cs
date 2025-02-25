using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Data
{
    public static class DbInitializer
    {
        public static void Initialize(OrderDbContext context)
        {
            // Применение миграций
            context.Database.Migrate();

            // Проверяем, есть ли заказы в базе данных
            if (context.Orders.Any()) return; // Если заказы уже есть, ничего не делаем

            // Создание списка заказов
            var orders = new List<Order>
            {
                new Order
                {
                    UserId = 1,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending", // Статус по умолчанию
                    TotalPrice = 59.97m, // Примерная общая сумма заказа
                    TotalQuantity = 3 // Общее количество игр в заказе
                },
                new Order
                {
                    UserId = 2,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Completed", // Статус завершён
                    TotalPrice = 29.99m, // Примерная общая сумма заказа
                    TotalQuantity = 1 // Общее количество игр в заказе
                }
            };

            // Добавляем заказы в контекст
            context.Orders.AddRange(orders);
            context.SaveChanges(); // Сохраняем, чтобы получить их ID

            // Создание списка элементов заказов
            var orderItems = new List<OrderItem>
            {
                new OrderItem { OrderId = orders[0].Id, GameId = 1, GameName = "Elden Ring", Price = 19.99m },
                new OrderItem { OrderId = orders[0].Id, GameId = 2, GameName = "Cyberpunk", Price = 19.99m },
                new OrderItem { OrderId = orders[1].Id, GameId = 3, GameName = "CS", Price = 29.99m }
            };

            // Добавляем элементы заказов в контекст
            context.OrderItems.AddRange(orderItems);
            context.SaveChanges(); // Сохраняем OrderItems

            // Обновляем общий total price и total quantity для каждого заказа
            foreach (var order in orders)
            {
                var items = orderItems.Where(oi => oi.OrderId == order.Id).ToList();
                order.TotalPrice = items.Sum(oi => oi.Price); // Пересчитываем цену
                order.TotalQuantity = items.Count; // Пересчитываем количество
            }

            // Обновляем заказы с правильной общей суммой и количеством
            context.Orders.UpdateRange(orders);
            context.SaveChanges();
        }
    }
}
