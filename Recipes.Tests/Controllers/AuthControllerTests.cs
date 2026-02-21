using Moq;
using Microsoft.AspNetCore.Mvc;
using Recipes.Controllers;
using Recipes.Interfaces;
using Recipes.Dtos.Requests;
using Recipes.Dtos.Responses;

namespace Recipes.Tests.Controllers
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsToken()
        {
            var mockTokenService = new Mock<ITokenService>();
            var request = new TokenRequest { ClientId = "test", ClientSecret = "secret" };
            var response = new TokenResponse
            (
                "mocked.jwt.token",
                10000,
                "Brearer"
            );
            mockTokenService.Setup(s => s.GenerateToken(request.ClientId, request.ClientSecret)).ReturnsAsync(response);

            var controller = new AuthController(mockTokenService.Object);
            var result = await controller.GetToken(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task Authenticate_InvalidCredentials_ReturnsUnauthorized()
        {
            var mockTokenService = new Mock<ITokenService>();
            var request = new TokenRequest { ClientId = "bad", ClientSecret = "wrong" };

            mockTokenService.Setup(s => s.GenerateToken(request.ClientId, request.ClientSecret)).ThrowsAsync(new UnauthorizedAccessException());

            var controller = new AuthController(mockTokenService.Object);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.GetToken(request));
        }
    }
}
