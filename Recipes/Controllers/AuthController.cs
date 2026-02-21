using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Dtos.Requests;
using Recipes.Interfaces;

namespace Recipes.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var result = await _tokenService.GenerateToken(request.ClientId, request.ClientSecret);

        if (result == null)
        {
            throw new UnauthorizedAccessException("Invalid client credentials.");
        }

        return Ok(result);
    }
}