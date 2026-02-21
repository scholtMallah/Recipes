using System.Diagnostics.CodeAnalysis;
using Recipes.Dtos.HelperTypes;

namespace Recipes.Dtos.Responses;

public class GetAllRecipesResponse
{
    public required IReadOnlyList<RecipeDto> Recipes { get; init; }
    public required int Page { get; init; }
    public required int Size { get; init; }
    public required int TotalPages { get; init; }
    
    [SetsRequiredMembers]
    public GetAllRecipesResponse(
        IReadOnlyList<RecipeDto> recipes,
        int page,
        int size,
        int totalPages)
    {
        Recipes = recipes;
        Page = page;
        Size = size;
        TotalPages = totalPages;
    }
}
