using Recipes.Dtos.Responses;

namespace Recipes.Interfaces;

public interface ITokenService
{
    public Task<TokenResponse?> GenerateToken(string clientId, string clientSecret);
}