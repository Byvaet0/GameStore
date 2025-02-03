using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    // Новый внешний ключ для связи с таблицей Roles
    public int RoleId { get; set; }

    // Навигационное свойство для связи с таблицей Roles
    public Role Role { get; set; } = null!;
}
