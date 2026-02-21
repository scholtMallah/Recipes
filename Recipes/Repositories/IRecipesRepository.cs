using RecipeModel = Recipes.Models.Recipes;

namespace Recipes.Repositories;

public interface IRecipesRepository
{
    Task<IReadOnlyList<RecipeModel>> GetAllAsync(CancellationToken ct = default);
    Task<RecipeModel?> GetByNameAsync(string name, CancellationToken ct = default);
    Task AddAsync(RecipeModel recipe, CancellationToken ct = default);
    Task UpdateAsync(RecipeModel recipe, CancellationToken ct = default);
    Task DeleteAsync(string name, CancellationToken ct = default);
}
