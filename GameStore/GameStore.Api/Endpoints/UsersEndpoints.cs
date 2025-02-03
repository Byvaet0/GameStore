using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class UsersEndpoints
{
    public static RouteGroupBuilder MapUsersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("users");

        // GET /users - получить список всех пользователей
        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            var users = await dbContext.Users
                .AsNoTracking() // Используем AsNoTracking для повышения производительности
                .Include(u => u.Role) // Включаем роль
                .ToListAsync();

            // Преобразуем список пользователей в DTO с RoleName
            var userDtos = users.Select(user => user.ToDto(dbContext)).ToList();

            return Results.Ok(userDtos);
        });

        // GET /users/{id} - Получить пользователя по ID
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var user = await dbContext.Users
                .Include(u => u.Role) // Включаем роль
                .FirstOrDefaultAsync(u => u.Id == id);

            return user is null ? Results.NotFound() : Results.Ok(user.ToDto(dbContext)); // Используем ToDto с dbContext
        });

       // POST /users - Создать нового пользователя
group.MapPost("/", async (UserRegistrationDto newUser, GameStoreContext dbContext) =>
{
    if (await dbContext.Users.AnyAsync(u => u.Username == newUser.Username))
    {
        return Results.BadRequest("Пользователь с таким именем уже существует.");
    }

    // Проверяем, существует ли роль с заданным RoleId
    var role = await dbContext.Roles.FindAsync(newUser.RoleId);
    if (role is null)
    {
        return Results.BadRequest("Роль с указанным ID не найдена.");
    }

    var user = new User
    {
        Username = newUser.Username,
        PasswordHash = newUser.Password, // Не забудьте хэшировать пароль перед сохранением
        RoleId = newUser.RoleId,
        Role = role // Присваиваем роль пользователю
    };

    dbContext.Add(user);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", user.ToDto(dbContext)); // Используем ToDto с dbContext
});


        // PUT /users/{id} - Обновить пользователя
        group.MapPut("/{id}", async (int id, UserDto updatedUser, GameStoreContext dbContext) =>
        {
            var existingUser = await dbContext.Users
                .Include(u => u.Role) // Включаем роль
                .FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser is null)
            {
                return Results.NotFound();
            }

            existingUser.Username = updatedUser.Username;
            existingUser.RoleId = updatedUser.RoleId;

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE /users/{id} - Удалить пользователя
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user is null)
            {
                return Results.NotFound();
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        return group;
    }
}
