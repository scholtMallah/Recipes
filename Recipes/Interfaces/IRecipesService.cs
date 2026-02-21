using Recipes.Dtos.HelperTypes;
using RecipeModel = Recipes.Models.Recipes;
using Recipes.QueryTypes;

namespace Recipes.Interfaces;

public interface IRecipesService
{
    Task<IReadOnlyList<string>> GetRecipeNames();
    Task<IngredientSummary> GetRecipeIngredientSummary(string name);

    Task<RecipeModel> GetRecipe(string name);

    Task<(IReadOnlyList<RecipeModel> Items, int Page, int Size, int TotalPages)> GetAllRecipes(GetAllRecipes query);
    
    Task<(IReadOnlyList<RecipeModel> Items, int Page, int Size, int TotalPages)> SearchRecipes(SearchRecipes query);
    
    Task CreateRecipe(RecipeModel recipe);
    Task UpdateRecipe(string name, RecipeModel recipe);
    Task PatchRecipe(string name, PatchRecipeCommand patch);

    Task DeleteAsync(string name);
}

public sealed class PatchRecipeCommand
{
    public List<string>? Ingredients { get; init; }
    public List<string>? Instructions { get; init; }
}