using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

// DTO для регистрации пользователя
public record class UserRegistrationDto(
    [Required][StringLength(50)] string Username,
    [Required][StringLength(100, MinimumLength = 4)] string Password,
    [Required] int RoleId
);