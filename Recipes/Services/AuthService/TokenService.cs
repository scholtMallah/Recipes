using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Recipes.Configuration;
using Recipes.Dtos.Responses;
using Recipes.Interfaces;

namespace Recipes.Services.AuthService;

public class TokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly List<ClientConfig> _clients;
    public TokenService(IOptions<JwtSettings> jwtOptions, IOptions<List<ClientConfig>> clientOptions)
    {
        _settings = jwtOptions.Value;
        _clients = clientOptions.Value;
    }

    public Task<TokenResponse?> GenerateToken(string clientId, string clientSecret)
    {
        var isValid = _clients.Any(c => c.ClientId == clientId && c.ClientSecret == clientSecret);
        if (!isValid) return Task.FromResult<TokenResponse?>(null);

        var claims = new[] { new Claim("client_id", clientId) };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.WriteToken(token);

        var response = new TokenResponse
        (
            jwt,
            (int)TimeSpan.FromMinutes(_settings.ExpiryMinutes).TotalSeconds,
            "Bearer"
        );

        return Task.FromResult<TokenResponse?>(response);
    }
}