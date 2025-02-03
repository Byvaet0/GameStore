using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

// DTO для авторизации пользователя
public record class UserLoginDto(
    [Required][StringLength(50)] string Username,
    [Required][StringLength(100, MinimumLength = 6)] string Password
);