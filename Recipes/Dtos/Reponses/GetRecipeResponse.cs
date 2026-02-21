using System.Diagnostics.CodeAnalysis;
using Recipes.Dtos.HelperTypes;

namespace Recipes.Dtos.Responses;

public class GetRecipeResponse
{
    public required RecipeDto Recipe { get; init; }
    
    [SetsRequiredMembers]
    public GetRecipeResponse(RecipeDto recipe)
    {
        Recipe = recipe;
    }
}
