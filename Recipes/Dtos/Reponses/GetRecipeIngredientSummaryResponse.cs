using System.Diagnostics.CodeAnalysis;
using Recipes.Dtos.HelperTypes;

namespace Recipes.Dtos.Responses;

public class GetRecipeIngredientSummaryResponse
{
    public required IngredientSummary Details { get; init; }
    
    [SetsRequiredMembers]
    public GetRecipeIngredientSummaryResponse(IngredientSummary details)
    {
        Details = details;
    }
}
