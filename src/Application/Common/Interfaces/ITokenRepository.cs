using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ITokenRepository : IRepositoryBase<Token>
{
    Task<Token?> GetTokenIncludeUserAsync(string accessToken, string refreshToken);
    Task<Token?> GetTokenByRefreshTokenAsync(string refreshToken);
    Task<Token?> GetTokenByJwtTokenAsync(string jwtToken);
}