using Recipes.Dtos.HelperTypes;
using Recipes.Interfaces;
using RecipeModel = Recipes.Models.Recipes;
using Recipes.QueryTypes;
using Recipes.Repositories;

namespace Recipes.Services.Recipes;

public class RecipesService : IRecipesService
{
    private readonly IRecipesRepository _repo;

    public RecipesService(IRecipesRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<string>> GetRecipeNames()
    {
        var all = await _repo.GetAllAsync();
        return all
            .Select(r => r.Name)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<IngredientSummary> GetRecipeIngredientSummary(string name)
    {
        var recipe = await _repo.GetByNameAsync(name)
                     ?? throw new KeyNotFoundException($"Recipe '{name}' not found.");

        return new IngredientSummary
        {
            Ingredients = recipe.Ingredients,
            NumSteps = recipe.Instructions.Count
        };
    }

    public async Task<RecipeModel> GetRecipe(string name)
    {
        return await _repo.GetByNameAsync(name)
               ?? throw new KeyNotFoundException($"Recipe '{name}' not found.");
    }

    public async Task<(IReadOnlyList<RecipeModel> Items, int Page, int Size, int TotalPages)> GetAllRecipes(GetAllRecipes query)
    {
        var all = await _repo.GetAllAsync();
        var total = all.Count;

        var size = Math.Max(1, query.Limit);
        var page = (query.Offset / size) + 1;
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var items = all
            .OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToList();

        return (items, page, size, totalPages);
    }


    public async Task<(IReadOnlyList<RecipeModel> Items, int Page, int Size, int TotalPages)> SearchRecipes(SearchRecipes query)
    {
        var q = (query.Query ?? "").Trim();

        var all = await _repo.GetAllAsync();

        IEnumerable<RecipeModel> filtered = all;

        if (!string.IsNullOrWhiteSpace(q))
        {
            filtered = all.Where(r =>
                r.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                (r.Ingredients?.Any(i => i.Contains(q, StringComparison.OrdinalIgnoreCase)) ?? false)
            );
        }

        var filteredList = filtered
            .OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var total = filteredList.Count;
        var size = Math.Max(1, query.Limit);
        var page = (query.Offset / size) + 1;
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var items = filteredList
            .Skip(query.Offset)
            .Take(query.Limit)
            .ToList();

        return (items, page, size, totalPages);
    }

    public async Task CreateRecipe(RecipeModel recipe)
    {
        recipe.Name = recipe.Name.Trim();
        var existing = await _repo.GetByNameAsync(recipe.Name);
        if (existing is not null)
            throw new InvalidOperationException($"Recipe '{recipe.Name}' already exists.");

        await _repo.AddAsync(recipe);
    }

    public async Task UpdateRecipe(string name, RecipeModel recipe)
    {
        if (!string.Equals(name, recipe.Name, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Route name does not match request body name.");

        recipe.Name = recipe.Name.Trim();
        await _repo.UpdateAsync(recipe);
    }

    public async Task PatchRecipe(string name, PatchRecipeCommand patch)
    {
        if (patch.Ingredients is null && patch.Instructions is null)
            throw new ArgumentException("At least one field must be provided for patch.");

        var existing = await _repo.GetByNameAsync(name)
                       ?? throw new KeyNotFoundException($"Recipe '{name}' not found.");

        var updated = new RecipeModel
        {
            Name = existing.Name,
            Ingredients = patch.Ingredients ?? existing.Ingredients,
            Instructions = patch.Instructions ?? existing.Instructions
        };

        await _repo.UpdateAsync(updated);
    }

    public Task DeleteAsync(string name) => _repo.DeleteAsync(name);
}