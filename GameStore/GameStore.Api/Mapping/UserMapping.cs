using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping
{
    public static class UserMapping
    {
        // Преобразование сущности User в UserDto
        public static UserDto ToDto(this User user, GameStoreContext dbContext)
        {
            return new UserDto(
                user.Id,
                user.Username,
                user.RoleId, // Добавляем RoleId
                user.Role.Name // Получаем название роли
            );
        }

        // Преобразование DTO в сущность User
        public static User ToEntity(this UserRegistrationDto dto)
        {
            return new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                RoleId = dto.RoleId
            };
        }

        // Хеширование пароля
        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    } 
}
