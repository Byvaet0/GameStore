using CartService.Entities;
using CartService.DTOs;
using CartService.Mapping;
using Microsoft.EntityFrameworkCore;
using CartService.Data;
using CartService.Services;
using System.Text.Json;

namespace CartService.Endpoints
{
    public static class CartEndpoints
    {
        public static void MapCartEndpoints(this WebApplication app)
        {
            var cartGroup = app.MapGroup("/cart");
            var rabbitMqService = new RabbitMqPublisher(); // Создаем экземпляр RabbitMqService

            // GET Получение корзины по UserId
            cartGroup.MapGet("/{userId}", async (int userId, CartDbContext db) =>
            {
                var cart = await db.Carts
                    .AsNoTracking()
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return Results.NotFound();

                return Results.Ok(CartMapping.ToDto(cart));
            });

            // 1. POST: Добавление товаров в корзину 
            cartGroup.MapPost("/add/{userId}", async (int userId, CartDto cartDto, CartDbContext db) =>
{
    var cart = await db.Carts
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    if (cart == null)
    {
        cart = new Cart { UserId = userId };
        db.Carts.Add(cart);
    }

    // Добавляем новые элементы в корзину
    foreach (var itemDto in cartDto.Items)
    {
        var item = CartItemMapping.ToEntity(itemDto);
        cart.Items.Add(item);
    }

    // Обновляем количество и общую цену
    cart.TotalQuantity = cart.Items.Count;
    cart.TotalPrice = cart.Items.Sum(item => item.Price);

    await db.SaveChangesAsync();

    return Results.Ok(CartMapping.ToDto(cart));
});


            // 2. POST: Отправка данных в OrderService
            cartGroup.MapPost("/checkout/{userId}", async (int userId, CartDbContext db) =>
            {
                var cart = await db.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.Items.Any())
                    return Results.BadRequest("Корзина пуста");

                var cartDto = CartMapping.ToDto(cart);  // Используем CartDto
                var cartJson = JsonSerializer.Serialize(cartDto);

                rabbitMqService.SendMessage("orderQueue", cartJson); // Отправка в OrderService

                // Очистка корзины после оформления заказа
                db.Carts.Remove(cart);
                await db.SaveChangesAsync();

                return Results.Ok("Заказ отправлен в обработку, корзина очищена");
            });

            // DELETE Удаление корзины по UserId
            cartGroup.MapDelete("/{userId}", async (int userId, CartDbContext db) =>
            {
                var cart = await db.Carts
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return Results.NotFound();

                db.Carts.Remove(cart);
                await db.SaveChangesAsync();

                return Results.Ok();
            });
        }
    }
}
