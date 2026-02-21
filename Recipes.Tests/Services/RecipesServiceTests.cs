using Microsoft.AspNetCore.Hosting;
using Moq;
using Recipes.Data;
using Recipes.Dtos.HelperTypes;
using Recipes.Interfaces;
using Recipes.Repositories;
using Recipes.Services.Recipes;
using RecipeModel = Recipes.Models.Recipes;

namespace Recipes.Tests.Services;

public class RecipesServiceTests
{
    private static (DbContext db, EfRecipeRepo repo, RecipesService service, string rootPath) CreateSut()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "recipes-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(Path.Combine(tempRoot, "Data"));

        var source = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Recipes", "Data", "data.json");
        source = Path.GetFullPath(source);
        File.Copy(source, Path.Combine(tempRoot, "Data", "data.json"), overwrite: true);

        var env = new Mock<IWebHostEnvironment>();
        env.Setup(e => e.ContentRootPath).Returns(tempRoot);

        var db = new DbContext(env.Object);
        var repo = new EfRecipeRepo(db);

        var service = new RecipesService(repo);

        return (db, repo, service, tempRoot);
    }

    [Fact]
    public async Task GetRecipeNames_ReturnsSeededNames()
    {
        var (_, _, service, root) = CreateSut();
        try
        {
            var names = await service.GetRecipeNames();
            Assert.Contains("scrambledEggs", names);
            Assert.Contains("garlicPasta", names);
            Assert.Contains("chai", names);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task GetRecipeIngredientSummary_ReturnsIngredientsAndNumSteps()
    {
        var (_, _, service, root) = CreateSut();
        try
        {
            IngredientSummary summary = await service.GetRecipeIngredientSummary("garlicPasta");
            Assert.Equal(5, summary.NumSteps);
            Assert.Contains("100g spaghetti", summary.Ingredients);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task CreateUpdatePatchDelete_WorksEndToEnd()
    {
        var (_, _, service, root) = CreateSut();
        try
        {
            await service.CreateRecipe(new RecipeModel
            {
                Name = "butteredBagel",
                Ingredients = new List<string> { "1 bagel", "butter" },
                Instructions = new List<string> { "cut the bagel", "spread butter on bagel" }
            });

            var namesAfterCreate = await service.GetRecipeNames();
            Assert.Contains("butteredBagel", namesAfterCreate);

            await service.UpdateRecipe("butteredBagel", new RecipeModel
            {
                Name = "butteredBagel",
                Ingredients = new List<string> { "1 bagel", "2 tbsp butter" },
                Instructions = new List<string> { "cut the bagel", "spread butter on bagel" }
            });

            var updated = await service.GetRecipe("butteredBagel");
            Assert.Contains("2 tbsp butter", updated.Ingredients);

            await service.PatchRecipe("butteredBagel", new PatchRecipeCommand
            {
                Ingredients = new List<string> { "1 bagel", "salted butter" }
            });

            var patched = await service.GetRecipe("butteredBagel");
            Assert.Contains("salted butter", patched.Ingredients);

            await service.DeleteAsync("butteredBagel");

            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetRecipe("butteredBagel"));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task CreateRecipe_DuplicateName_ThrowsInvalidOperationException()
    {
        var (_, _, service, root) = CreateSut();
        try
        {
            await service.CreateRecipe(new RecipeModel
            {
                Name = "dupRecipe",
                Ingredients = new List<string> { "a" },
                Instructions = new List<string> { "b" }
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateRecipe(new RecipeModel
                {
                    Name = "dupRecipe",
                    Ingredients = new List<string> { "x" },
                    Instructions = new List<string> { "y" }
                }));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task SearchRecipes_FiltersByNameOrIngredients_AndPages()
    {
        var (_, _, service, root) = CreateSut();
        try
        {
            var (items, page, size, totalPages) = await service.SearchRecipes(new QueryTypes.SearchRecipes
            {
                Query = "garlic",
                Offset = 0,
                Limit = 10
            });

            Assert.True(items.Count >= 1);
            Assert.Contains(items, r => r.Name.Equals("garlicPasta", StringComparison.OrdinalIgnoreCase));

            Assert.Equal(1, page);
            Assert.Equal(10, size);
            Assert.True(totalPages >= 1);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}