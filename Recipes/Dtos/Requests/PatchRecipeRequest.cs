using System.ComponentModel.DataAnnotations;

namespace Recipes.Dtos.Requests;

public class PatchRecipeRequest
{
    [MinLength(1)]
    public List<string>? Ingredients { get; init; }

    [MinLength(1)]
    public List<string>? Instructions { get; init; }
}
