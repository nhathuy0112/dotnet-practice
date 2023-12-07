using Application.Common.Interfaces;
using Application.Common.Services;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UnitTest.Service;

public class TokenServiceTest
{
    private readonly Mock<IUserRepository> _mockUserRepo = new();
    private readonly Mock<ITokenRepository> _mockTokenRepo = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IConfiguration> _mockConfig = new();

    public TokenServiceTest()
    {
        SetupMockUnitOfWork();
        SetupMockConfig();
    }

    [Fact]
    public async Task Test_CreateAccessTokenAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();

        _mockUserRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>());
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);
        
        var user = new AppUser()
        {
            Id = "abcd123q4",
            Email = "abc@abc.com"
        };
        
        // ACT
        var data = await tokenService.CreateAccessTokenAsync(user);
        
        // ASSERT
        _mockUserRepo.Verify(x => x.GetRolesAsync(It.IsAny<AppUser>()), onceTime);
        Assert.NotEqual(0, data.Length);
    }

    [Fact]
    public void Test_CreateRefreshToken()
    {
        // ARRANGE
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        var data = tokenService.CreateRefreshToken();
        
        // ASSERT
        Assert.NotEqual(0, data.Length);
    }

    [Fact]
    public async Task Test_CreateTokensAsync()
    {
        // ARRANGE
        _mockUserRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>());
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        var user = new AppUser()
        {
            Id = "abcd123q4",
            Email = "abc@abc.com"
        };

        var expire = int.Parse(_mockConfig.Object["Jwt:RefreshTokenExpirationTime"]);

        var refreshExpire = DateTime.Now.AddDays(expire);
        
        // ACT
        var data = await tokenService.CreateTokensAsync(user);
        
        // ASSERT
        Assert.Equal("abcd123q4", data.UserId);
        Assert.NotEqual(0, data.JwtToken.Length);
        Assert.NotEqual(0, data.RefreshToken.Length);
        Assert.Equal(refreshExpire.Date, data.RefreshExpiryDate.Date);
    }

    [Fact]
    public async Task Test_SaveTokenAsync()
    {
        // ARRANGE
        var onceTime = Times.Once;
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);
        
        // ACT
        await tokenService.SaveTokenAsync(new Token());
        
        // ASSERT
        _mockTokenRepo.Verify(x => x.AddTokenAsync(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), onceTime);
    }

    [Fact]
    public async Task Test_ValidateTokenAsync_Valid()
    {
        // ARRANGE
        var expire = int.Parse(_mockConfig.Object["Jwt:RefreshTokenExpirationTime"]);
        var refreshExpire = DateTime.Now.AddDays(expire);
        var tokenNotExpire = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };

        _mockTokenRepo
            .Setup(x => x.GetTokenByRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(tokenNotExpire);
                
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        var data = await tokenService.ValidateTokenAsync("");
        
        // ASSERT
        Assert.Equal(tokenNotExpire, data);
    }

    [Fact]
    public async Task Test_ValidateTokenAsync_Token_Not_Found()
    {
        // ARRANGE
        _mockTokenRepo
            .Setup(x => x.GetTokenByRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((Token?)null);
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        var data = await tokenService.ValidateTokenAsync("");
        
        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_ValidateTokenAsync_Token_Expire()
    {
        // ARRANGE
        var refreshExpire = DateTime.Now.Subtract(TimeSpan.FromDays(1));
        var tokenExpire = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockTokenRepo
            .Setup(x => x.GetTokenByRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(tokenExpire);
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        var data = await tokenService.ValidateTokenAsync("");
        
        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_UpdateTokenAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        await tokenService.UpdateTokenAsync(new Token());

        // ASSERT
        _mockTokenRepo.Verify(x => x.UpdateTokenAsync(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), onceTime);
    }
    
    [Fact]
    public async Task Test_DeleteTokenAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        await tokenService.DeleteTokenAsync(new Token());

        // ASSERT
        _mockTokenRepo.Verify(x => x.DeleteTokenAsync(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), onceTime);
    }

    [Fact]
    public async Task Test_ValidateTokensAsync_Valid()
    {
        // ARRANGE
        var expire = int.Parse(_mockConfig.Object["Jwt:RefreshTokenExpirationTime"]);
        var refreshExpire = DateTime.Now.AddDays(expire);
        var tokenNotExpire = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockUserRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>());
        
        _mockConfig
            .SetupGet(x => x[It.Is<string>(s => s == "Jwt:AccessTokenExpirationTime")])
            .Returns("0");
        
        _mockTokenRepo
            .Setup(x => x.GetTokenIncludeUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(tokenNotExpire);

        var user = new AppUser()
        {
            Id = "123123nmas",
            Email = "abc@abc.com"
        };

        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);
        
        var expiredAccessToken = await tokenService.CreateAccessTokenAsync(user);

        // ACT
        var data = await tokenService.ValidateTokensAsync(expiredAccessToken, "");
        
        // ASSERT
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Test_ValidateTokensAsync_AccessToken_Not_Expire()
    {
        // ARRANGE
        var expire = int.Parse(_mockConfig.Object["Jwt:RefreshTokenExpirationTime"]);
        var refreshExpire = DateTime.Now.AddDays(expire);
        var tokenNotExpire = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockTokenRepo
            .Setup(x => x.GetTokenIncludeUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(tokenNotExpire);
        
        _mockUserRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>());
        
        var user = new AppUser()
        {
            Id = "123123nmas",
            Email = "abc@abc.com"
        };

        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);
        
        var aliveAccessToken = await tokenService.CreateAccessTokenAsync(user);

        // ACT
        var data = await tokenService.ValidateTokensAsync(aliveAccessToken, "");
        
        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_ValidateTokensAsync_Token_NotFound()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        _mockUserRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>());
        
        _mockConfig
            .SetupGet(x => x[It.Is<string>(s => s == "Jwt:AccessTokenExpirationTime")])
            .Returns("0");
        
        _mockTokenRepo
            .Setup(x => x.GetTokenIncludeUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Token?)null);

        var user = new AppUser()
        {
            Id = "123123nmas",
            Email = "abc@abc.com"
        };

        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);
        
        var expiredAccessToken = await tokenService.CreateAccessTokenAsync(user);

        // ACT
        var data = await tokenService.ValidateTokensAsync(expiredAccessToken, "");
        
        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_ValidationTokensAsync_RefreshToken_Expired()
    {
        // ARRANGE
        var refreshExpire = DateTime.Now.Subtract(TimeSpan.FromDays(1));
        var expiredToken = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockUserRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>());
        
        _mockConfig
            .SetupGet(x => x[It.Is<string>(s => s == "Jwt:AccessTokenExpirationTime")])
            .Returns("0");
        
        _mockTokenRepo
            .Setup(x => x.GetTokenIncludeUserAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expiredToken);

        var user = new AppUser()
        {
            Id = "123123nmas",
            Email = "abc@abc.com"
        };

        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);
        
        var expiredAccessToken = await tokenService.CreateAccessTokenAsync(user);

        // ACT
        var data = await tokenService.ValidateTokensAsync(expiredAccessToken, "");
        
        // ASSERT
        Assert.Null(data);
    }

    private void SetupMockUnitOfWork()
    {
        _mockUnitOfWork
            .Setup(x => x.TokenRepository)
            .Returns(_mockTokenRepo.Object);
    }

    private void SetupMockConfig()
    {
        _mockConfig
            .SetupGet(x => x[It.Is<string>(s => s == "Jwt:AccessTokenExpirationTime")])
            .Returns("1");
        _mockConfig
            .SetupGet(x => x[It.Is<string>(s => s == "Jwt:RefreshTokenExpirationTime")])
            .Returns("90");
        _mockConfig
            .SetupGet(x => x[It.Is<string>(s => s == "Jwt:AccessTokenSecret")])
            .Returns("Mock token secret key");
    }

}