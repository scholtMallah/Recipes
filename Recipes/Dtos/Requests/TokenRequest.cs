using System.ComponentModel.DataAnnotations;

namespace Recipes.Dtos.Requests;

public class TokenRequest
{
    [Required]
    public string ClientId { get; set; } = null!;
    [Required]
    public string ClientSecret { get; set; } = null!;
}