using System.ComponentModel.DataAnnotations;

namespace Recipes.Dtos.Requests;

public class CreateOrPutRecipeRequest
{
    [Required]
    public required string Name { get; init; }

    [Required]
    [MinLength(1)]
    public required List<string> Ingredients { get; init; }

    [Required]
    [MinLength(1)]
    public required List<string> Instructions { get; init; }
}
