using Recipes.Services.AuthService;
using Recipes.Configuration;
using Microsoft.Extensions.Options;
using Recipes.Dtos.Responses;

namespace Recipes.Tests.Services
{
    public class TokenServiceTests
    {
        [Fact]
        public async Task GenerateToken_ReturnsValidToken()
        {
            var settings = Options.Create(new JwtSettings
            {
                Secret = "supersecurekey12345678901234567890",
                ExpiryMinutes = 30
            });
            var clientSettings = Options.Create(new List<ClientConfig>{
                new ClientConfig(){
                    ClientId = "test",
                    ClientSecret = "secret"
                },
            });

            var service = new TokenService(settings, clientSettings);
            var token = await service.GenerateToken("test", "secret");

            Assert.IsType<TokenResponse>(token);
        }
    }
}
