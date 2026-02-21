namespace Recipes.Dtos.HelperTypes;

public class RecipeDto
{
    public required string Name { get; init; }
    public required List<string> Ingredients { get; init; }
    public required List<string> Instructions { get; init; }
}
