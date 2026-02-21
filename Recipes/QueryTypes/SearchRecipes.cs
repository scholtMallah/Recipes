using Microsoft.AspNetCore.Mvc;

namespace Recipes.QueryTypes;

public class SearchRecipes
{
    [FromQuery(Name = "query")]
    public required string Query { get; set; }
    [FromQuery(Name = "offset")]
    public int Offset { get; init; } = 0;
    [FromQuery(Name = "limit")]
    public int Limit { get; init; } = 10;
}