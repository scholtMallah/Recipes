namespace Recipes.Dtos.Responses;

public class TokenResponse
{
    public string AccessToken { get; init; }
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; }
    
    public TokenResponse(
        string accessToken,
        int expiresIn,
        string tokenType)
    {
        AccessToken = accessToken;
        ExpiresIn = expiresIn;
        TokenType = tokenType;
    }
}