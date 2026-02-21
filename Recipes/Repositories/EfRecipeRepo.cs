using Recipes.Data;
using RecipeModel = Recipes.Models.Recipes;

namespace Recipes.Repositories;

// neppe ef repo
public class EfRecipeRepo : IRecipesRepository
{
    private static readonly SemaphoreSlim _writeLock = new(1, 1);

    private readonly DbContext _db;

    public EfRecipeRepo(DbContext db)
    {
        _db = db;
    }

    public Task<IReadOnlyList<RecipeModel>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult((IReadOnlyList<RecipeModel>)_db.Recipes.ToList());
    }

    public Task<RecipeModel?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var recipe = _db.Recipes.FirstOrDefault(r =>
            string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(recipe);
    }

    public async Task AddAsync(RecipeModel recipe, CancellationToken ct = default)
    {
        await _writeLock.WaitAsync(ct);
        try
        {
            _db.Load();

            if (_db.Recipes.Any(r => string.Equals(r.Name, recipe.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Recipe '{recipe.Name}' already exists.");

            _db.Recipes.Add(recipe);
            await _db.SaveAsync(ct);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task UpdateAsync(RecipeModel recipe, CancellationToken ct = default)
    {
        await _writeLock.WaitAsync(ct);
        try
        {
            _db.Load();

            var idx = _db.Recipes.FindIndex(r =>
                string.Equals(r.Name, recipe.Name, StringComparison.OrdinalIgnoreCase));

            if (idx < 0)
                throw new KeyNotFoundException($"Recipe '{recipe.Name}' not found.");

            _db.Recipes[idx] = recipe;
            await _db.SaveAsync(ct);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    public async Task DeleteAsync(string name, CancellationToken ct = default)
    {
        await _writeLock.WaitAsync(ct);
        try
        {
            _db.Load();

            var existing = _db.Recipes.FirstOrDefault(r =>
                string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
                throw new KeyNotFoundException($"Recipe '{name}' not found.");

            _db.Recipes.Remove(existing);
            await _db.SaveAsync(ct);
        }
        finally
        {
            _writeLock.Release();
        }
    }
}