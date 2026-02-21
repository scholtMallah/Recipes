using Microsoft.AspNetCore.Mvc;

namespace Recipes.QueryTypes;

public sealed class GetAllRecipes
{
    [FromQuery(Name = "offset")]
    public int Offset { get; init; } = 0;
    [FromQuery(Name = "limit")]
    public int Limit  { get; init; } = 20;
}
