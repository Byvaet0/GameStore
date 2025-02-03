using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

// DTO для представления жанра
public record class GenreDto(
    int Id,
    [Required][StringLength(50)] string Name
);
