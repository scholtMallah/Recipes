namespace Recipes.Dtos.Responses;

public class GetRecipeNamesResponse
{
    public List<string> RecipeNames { get; init; }
    public GetRecipeNamesResponse(List<string> recipeNames)
    {
        RecipeNames = recipeNames;
    }
}