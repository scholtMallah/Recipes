namespace Recipes.Models;

public class Recipes
{
    required
    public string Name { get; set; }
    required
    public List<string> Ingredients { get; set; }
    required
    public List<string> Instructions { get; set; }
}