using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Domain.Specification.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Application.Services;

public class TokenService : ITokenService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DateTime _now;
    public TokenService(IUserRepository userRepository, IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _now = DateTime.Now.ToUniversalTime();
        _userRepository = userRepository;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> CreateAccessTokenAsync(AppUser user)
    {
        var claims = await CreateClaimsAsync(user);
        var tokenDescriptor = CreateTokenDescriptor(claims);
        var tokenHandler = new JsonWebTokenHandler();
        var accessToken = tokenHandler.CreateToken(tokenDescriptor);
        return accessToken;
    }

    private async Task<List<Claim>> CreateClaimsAsync(AppUser user)
    {
        var claims = new List<Claim>()
        {
            new Claim("Id", user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        var roles = await _userRepository.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(List<Claim> claims)
    {
        double expire = double.Parse(_configuration["Jwt:AccessTokenExpirationTime"]);
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:AccessTokenSecret"]));
                    
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(expire),
            SigningCredentials = credentials,
        };
        return tokenDescriptor;
    }

    public string CreateRefreshToken()
    {
        var random = new Random();
        var guid = Guid.NewGuid();
        var chars = guid + "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" + DateTime.Now.ToShortDateString();
        return new string(
            Enumerable.
                Repeat(chars, 100).
                Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<Token> CreateTokensAsync(AppUser user)
    {
        string accessToken = await CreateAccessTokenAsync(user);
        string refreshToken = CreateRefreshToken();
        int expire = int.Parse(_configuration["Jwt:RefreshTokenExpirationTime"]);
        var createdDate = DateTime.Now;
        var expiryDate = DateTime.Now.AddDays(expire);
        var tokenInfo = new Token()
        {
            UserId = user.Id,
            JwtToken = accessToken,
            RefreshToken = refreshToken,
            RefreshCreatedDate = createdDate,
            RefreshExpiryDate = expiryDate
        };
        return tokenInfo;
    }

    public async Task SaveTokenAsync(Token tokenEntity)
    {
        _unitOfWork.Repository<Token>().Add(tokenEntity);
        await _unitOfWork.CompleteAsync();
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        return new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:AccessTokenSecret"])),
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
        };
    }

    private bool IsAlgorithmValid(SecurityToken validatedToken)
    {
        if (validatedToken is not JsonWebToken jwtSecurityToken)
        {
            return true;
        };
        var result = jwtSecurityToken.Alg.Equals(SecurityAlgorithms.HmacSha512Signature);
        return result;
    }

    private bool IsAccessTokenExpired(IEnumerable<Claim> claims)
    {
        var tokenExp = claims
            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!
            .Value;
        var tokenTicks = long.Parse(tokenExp);
        var expiryDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;
        return expiryDate <= DateTime.Now.ToUniversalTime();
    }
    
    private bool IsAccessTokenValid(string accessToken)
    {
        var tokenValidationParams = GetTokenValidationParameters();
        var tokenHandler = new JsonWebTokenHandler();
        var tokenClaimsPrincipal = tokenHandler.ValidateToken(
            token: accessToken,
            validationParameters: tokenValidationParams);
        
        var isAlgorithmValid = IsAlgorithmValid(tokenClaimsPrincipal.SecurityToken);
        
        if (!isAlgorithmValid)
        {
            return false;
        }

        var isExpired = IsAccessTokenExpired(tokenClaimsPrincipal.ClaimsIdentity.Claims);

        return isExpired;
    }

    private async Task<Token?> GetTokenIncludeUserAsync(string accessToken, string refreshToken)
    {
        var tokenWithUserSpec = new TokenSpecification(accessToken: accessToken, refreshToken: refreshToken);
        var tokenInfo = await _unitOfWork.Repository<Token>().GetAsync(tokenWithUserSpec);
        return tokenInfo;
    }

    public async Task<Token?> ValidateTokensAsync(string accessToken, string refreshToken)
    {
        var isAccessTokenValid = IsAccessTokenValid(accessToken);
        
        if (!isAccessTokenValid)
        {
            return null;
        }

        var token = await GetTokenIncludeUserAsync(accessToken: accessToken, refreshToken: refreshToken);

        if (token == null)
        {
            return null;
        }
        
        var isTokenExpired = token.RefreshExpiryDate >= DateTime.Now;

        return !isTokenExpired ? null : token;
    }

    public async Task<Token?> ValidateTokenAsync(string refreshToken)
    {
        var tokenSpec = new TokenSpecification(refreshToken: refreshToken);
        var token = await _unitOfWork.Repository<Token>().GetAsync(tokenSpec);
        
        if (token == null)
        {
            return null;
        }

        return token.RefreshExpiryDate < DateTime.Now ? null : token;
    }

    public async Task UpdateTokenAsync(Token tokenEntity)
    {
        _unitOfWork.Repository<Token>().Update(tokenEntity);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteTokenAsync(Token tokenEntity)
    {
        _unitOfWork.Repository<Token>().Delete(tokenEntity);
        await _unitOfWork.CompleteAsync();
    }
}