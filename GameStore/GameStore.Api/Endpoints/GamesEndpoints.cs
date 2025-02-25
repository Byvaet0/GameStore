using System;
using System.Text.Json;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using GameStore.Api.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Services;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGame";
    const string GamesCacheKey = "games_list"; // Ключ кэша для списка игр
  
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
{
    var group = app.MapGroup("games").WithParameterValidation();


    //GET /games
    group.MapGet("/", async (GameStoreContext dbContext, RedisService redisService) => 
    {
            // Проверяем, есть ли данные в Redis
            var cachedGames = await redisService.GetAsync(GamesCacheKey);
            if (cachedGames != null)
            {
                return Results.Ok(JsonSerializer.Deserialize<List<GameSummaryDto>>(cachedGames));
            }

            // Если в кэше нет данных, загружаем из БД
            var games = await dbContext.Games
                .Include(game => game.Genre)
                .Select(game => game.ToGameSummaryDto())
                .AsNoTracking()
                .ToListAsync();

            // Кэшируем список игр на 10 минут
            await redisService.SetAsync(GamesCacheKey, JsonSerializer.Serialize(games), TimeSpan.FromMinutes(10));

            return Results.Ok(games);
        });
      


    //GET /games/1
    group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => 
    {
        Game? game = await dbContext.Games.FindAsync(id);

        return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
    })
    .WithName(GetGameEndPointName);

  
    //POST /games
    group.MapPost("/", [Authorize(Roles = "Admin")] async (CreateGameDto newGame, GameStoreContext dbContext, RedisService redisService) => 
    {
      
        Game game = newGame.ToEntity();

        dbContext.Add(game);
        await dbContext.SaveChangesAsync();

         // Очищаем кэш, чтобы он обновился при следующем запросе
        await redisService.SetAsync(GamesCacheKey, "", TimeSpan.FromSeconds(1));

        return Results.CreatedAtRoute(GetGameEndPointName, new {id = game.Id}, game.ToGameDetailsDto());  //use game.ToDto
    });
    


    //PUT /games/1
    group.MapPut("/{id}", [Authorize(Roles = "Admin")] async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext, RedisService redisService) => 
    {
    var existingGame = await dbContext.Games.FindAsync(id);

    if (existingGame is null)
    {
     return Results.NotFound();
    }

        dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));
        await dbContext.SaveChangesAsync();

        // Очищаем кэш
        await redisService.SetAsync(GamesCacheKey, "", TimeSpan.FromSeconds(1));

         return Results.NoContent();
});


    //DELETE /games/1
    group.MapDelete("/{id}", [Authorize(Roles = "Admin")] async (int id, GameStoreContext dbContext, RedisService redisService) =>
    {
        await dbContext.Games.Where(game => game.Id == id).ExecuteDeleteAsync();

        // Очищаем кэш
        await redisService.SetAsync(GamesCacheKey, "", TimeSpan.FromSeconds(1));
    
        return Results.NoContent();
    });


     // POST  эндпоинт для добавления игры в корзину и отправки данных в RabbitMQ
          group.MapPost("/add-to-cart", [Authorize] async (AddToCartDto addToCartDto, GameStoreContext dbContext, RabbitMqService rabbitMqService) =>
{
    try
    {
        // Извлекаем игру по GameId
        var game = await dbContext.Games.FindAsync(addToCartDto.GameId);

        if (game == null)
        {
            return Results.NotFound(new { message = "Игра не найдена." });
        }

        // Публикуем данные о добавленной игре в очередь RabbitMQ
        rabbitMqService.PublishMessage(new
        {
            UserId = addToCartDto.UserId,
            GameId = addToCartDto.GameId,
            GameName = game.Name,  // Извлекаем название игры
            Price = game.Price     // Извлекаем цену игры
        });

        return Results.Ok(new { message = "Игра добавлена в корзину и передана в очередь." });
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = "Ошибка при отправке данных в очередь.", details = ex.Message }, statusCode: 500);
    }
});



        return group;
    }


    



}
