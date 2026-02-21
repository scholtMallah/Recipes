namespace Recipes.Dtos.HelperTypes;

public class IngredientSummary
{
    public required List<string> Ingredients { get; init; }
    public required int NumSteps { get; init; }
}
