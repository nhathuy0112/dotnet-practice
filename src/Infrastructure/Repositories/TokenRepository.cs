using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TokenRepository : RepositoryBase<Token>, ITokenRepository
{
    private readonly AppDbContext _context;
    public TokenRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Token?> GetTokenIncludeUserAsync(string accessToken, string refreshToken)
    {
        return await _context.Tokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.JwtToken == accessToken && t.RefreshToken == refreshToken);
    }

    public async Task<Token?> GetTokenByRefreshTokenAsync(string refreshToken)
    {
        return await Task.FromResult(_context.Tokens.FirstOrDefault(t => t.RefreshToken == refreshToken));
    }

    public async Task<Token?> GetTokenByJwtTokenAsync(string jwtToken)
    {
        return await Task.FromResult(_context.Tokens.FirstOrDefault(t => t.JwtToken == jwtToken));
    }
}