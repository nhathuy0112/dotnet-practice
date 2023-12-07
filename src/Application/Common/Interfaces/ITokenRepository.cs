using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITokenRepository
{
    Task AddTokenAsync(Token token);
    Task<Token?> GetTokenIncludeUserAsync(string accessToken, string refreshToken);
    Task<Token?> GetTokenByRefreshTokenAsync(string refreshToken);
    Task<Token?> GetTokenByJwtTokenAsync(string jwtToken);
    Task UpdateTokenAsync(Token token);
    Task DeleteTokenAsync(Token token);
}