using System.Text.Json;
using System.Text.Json.Serialization;
using RecipeModel = Recipes.Models.Recipes;

namespace Recipes.Data;

// neppe DB context om EF systemen na te maken
public class DbContext
{
    private readonly IWebHostEnvironment _env;
    private readonly string _jsonPath;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public List<RecipeModel> Recipes { get; private set; } = new();

    public DbContext(IWebHostEnvironment env)
    {
        _env = env;
        _jsonPath = Path.Combine(_env.ContentRootPath, "Data", "data.json");
        Load();
    }

    private sealed class DataRoot
    {
        [JsonPropertyName("recipes")]
        public List<RecipeModel> Recipes { get; set; } = new();
    }

    public void Load()
    {
        if (!File.Exists(_jsonPath))
        {
            Recipes = new List<RecipeModel>();
            return;
        }

        var json = File.ReadAllText(_jsonPath);
        var root = JsonSerializer.Deserialize<DataRoot>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Recipes = root?.Recipes ?? new List<RecipeModel>();
    }

    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _writeLock.WaitAsync(ct);
        try
        {
            var root = new DataRoot { Recipes = Recipes };

            var json = JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);

            var tmp = _jsonPath + ".tmp";
            await File.WriteAllTextAsync(tmp, json, ct);
            File.Move(tmp, _jsonPath, overwrite: true);
        }
        finally
        {
            _writeLock.Release();
        }
    }
}
