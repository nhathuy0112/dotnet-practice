using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace UnitTest.Repository;

public class TokenRepositoryTest
{
    private Mock<AppDbContext> _mockContext;
    private List<Token> _data;

    public TokenRepositoryTest()
    {
        SetupData();
        SetupContext();
    }

    [Fact]
    public async Task Test_GetTokenIncludeUserAsync()
    {
        // ARRANGE
        var tokenRepo = new TokenRepository(_mockContext.Object);
        string jwtToken = "jwt";
        string refreshToken = "refresh";
        string userId = "Abcd";
        
        // ACT
        var data = await tokenRepo.GetTokenIncludeUserAsync(accessToken: jwtToken, refreshToken: refreshToken);
        
        //
        Assert.Equal(userId, data.User.Id);
    }

    [Fact]
    public async Task Test_GetTokenByJwtTokenAsync()
    {
        // ARRANGE
        var tokenRepo = new TokenRepository(_mockContext.Object);
        string jwtToken = "jwt";
        
        // ACT
        var data = await tokenRepo.GetTokenByJwtTokenAsync(jwtToken);
        
        // ASSERT
        Assert.NotNull(data);
    }
    
    [Fact]
    public async Task Test_GetTokenByRefreshTokenAsync()
    {
        // ARRANGE
        var tokenRepo = new TokenRepository(_mockContext.Object);
        string refreshToken = "refresh";
        
        // ACT
        var data = await tokenRepo.GetTokenByRefreshTokenAsync(refreshToken);
        
        // ASSERT
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Test_AddTokenAsync()
    {
        // ARRANGE
        var newToken = new Token()
        {
            Id = 3
        };
        var tokenRepo = new TokenRepository(_mockContext.Object);

        // ACT
        await tokenRepo.AddTokenAsync(newToken);
        
        // ASSERT
        Assert.Equal(newToken.Id, _data[^1].Id);
    }

    [Fact]
    public async Task Test_DeleteTokenAsync()
    {
        // ARRANGE
        var countBeforeDelete = _data.Count;
        var tokenToDelete = _data[1];
        var tokenRepo = new TokenRepository(_mockContext.Object);

        // ACT
        await tokenRepo.DeleteTokenAsync(tokenToDelete);
        
        // ASSERT
        Assert.NotEqual(countBeforeDelete, _data.Count);
    }
    
    private void SetupData()
    {
        _data = new()
        {
            new()
            {
                Id = 1,
                User = new AppUser()
                {
                    Id = "Abcd"
                },
                JwtToken = "jwt",
                RefreshToken = "refresh"
            },
            new()
            {
                Id = 2,
            }
        };
    }

    private void SetupContext()
    {
        var options = new DbContextOptionsBuilder().Options;
        _mockContext = new(options);
        
        _mockContext
            .Setup(x => x.Tokens)
            .Returns(_data.AsQueryable().BuildMockDbSet().Object);

        _mockContext
            .Setup(x => x.Set<Token>())
            .Returns(_data.AsQueryable().BuildMockDbSet().Object);
        
        _mockContext
            .Setup(c => c.Set<Token>().Add(It.IsAny<Token>()))
            .Callback((Token token) => _data.Add(token));

        _mockContext
            .Setup(c => c.Set<Token>().Remove(It.IsAny<Token>()))
            .Callback((Token token) => _data.Remove(token));
    }
}