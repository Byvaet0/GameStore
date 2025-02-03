using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

// DTO для передачи информации о пользователе
public record class UserDto(
    int Id,
    [Required][StringLength(50)] string Username,
    [Required] int RoleId, // Добавлено поле RoleId
    string RoleName // Добавлено поле RoleName для отображения
);