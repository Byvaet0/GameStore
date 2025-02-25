using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Entities;
using OrderService.Mapping;

namespace OrderService.Endpoints
{
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this WebApplication app)
        {
            var orderGroup = app.MapGroup("/orders");

            // GET Получение всех заказов
            orderGroup.MapGet("/", async (OrderDbContext db) =>
            {
               var orders = await db.Orders.Include(o => o.OrderItems)
                            .Select(o => o.ToDto()) // Используем маппинг в DTO
                            .ToListAsync();
                return Results.Ok(orders);
            });

            // GET {id} Получение заказа по ID
            orderGroup.MapGet("/{id}", async (int id, OrderDbContext db) =>
            {
                var order = await db.Orders.Include(o => o.OrderItems)
                                           .FirstOrDefaultAsync(o => o.Id == id); 
                if (order is null)
                    return Results.NotFound();
                return Results.Ok(order.ToDto());
            });

            // POST Создание нового заказа
            orderGroup.MapPost("/", async (OrderDto orderDto, OrderDbContext db) =>
            {
                var order = new Order
                {
                    UserId = orderDto.UserId, 
                    CreatedAt = DateTime.UtcNow, 
                    Status = "Pending", 
                    TotalPrice = orderDto.OrderItems.Sum(oi => oi.Price),
                    TotalQuantity = orderDto.OrderItems.Count, 
                    OrderItems = orderDto.OrderItems.Select(oi => new OrderItem
                    {
                        GameId = oi.GameId,
                        GameName = oi.GameName, 
                        Price = oi.Price
                    }).ToList()
                };

                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return Results.Created($"/orders/{order.Id}", order.ToDto()); 
            });

            // PUT Обновление заказа
            orderGroup.MapPut("/{id}", async (int id, OrderDto orderDto, OrderDbContext db) =>
            {
                var order = await db.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id); 

                if (order == null)
                    return Results.NotFound();

                order.Status = orderDto.Status;
                order.TotalPrice = orderDto.OrderItems.Sum(oi => oi.Price);
                order.TotalQuantity = orderDto.OrderItems.Count; 

                order.OrderItems.Clear();
                order.OrderItems.AddRange(orderDto.OrderItems.Select(item => new OrderItem
                {
                    GameId = item.GameId,
                    GameName = item.GameName,
                    Price = item.Price
                }));

                db.Orders.Update(order);
                await db.SaveChangesAsync();
                return Results.Ok(order.ToDto()); 
            });

            // DELETE Удаление заказа
            orderGroup.MapDelete("/{id}", async (int id, OrderDbContext db) =>
            {
                var order = await db.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id); 
                if (order == null)
                    return Results.NotFound();

                db.OrderItems.RemoveRange(order.OrderItems); 
                db.Orders.Remove(order);
                await db.SaveChangesAsync();

                return Results.Ok();
            });
        }
    }
}
