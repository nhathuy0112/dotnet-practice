using Domain.Entities;

namespace Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateAccessTokenAsync(AppUser user);
    string CreateRefreshToken();
    Task<Token> CreateTokensAsync(AppUser user);
    Task SaveTokenAsync(Token tokenEntity);
    Task<Token?> ValidateTokensAsync(string accessToken, string refreshToken);
    Task<Token?> ValidateTokenAsync(string refreshToken);
    Task UpdateTokenAsync(Token tokenEntity);
    Task DeleteTokenAsync(Token tokenEntity);
}