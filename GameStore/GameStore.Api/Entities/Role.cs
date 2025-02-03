using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Entities;

public class Role
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    // Навигационное свойство — один ко многим
    public List<User> Users { get; set; } = new();
}
