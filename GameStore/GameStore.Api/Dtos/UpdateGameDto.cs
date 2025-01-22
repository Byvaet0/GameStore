using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateGameDto(
    [Required][StringLength(50)] string Name,
   [Required] int GenreId,
   [Required][Range(0, 1000)] decimal Price,
    DateOnly ReleaseDate
    );

