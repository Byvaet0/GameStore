using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

// DTO для представления роли
public record class RoleDto(
    int Id,
    [Required][StringLength(20)] string Name
);
