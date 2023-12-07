using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Specification;
using Microsoft.Extensions.Configuration;
using Moq;
using UnitTest.Service.Setup;

namespace UnitTest.Service;

public class TokenTest
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IRepository<Token>> _mockTokenRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IConfiguration> _mockConfig;

    public TokenTest()
    {
        _mockUserRepo = UserMocker.GetMockRepo();
        _mockTokenRepo = Mocker.GetMockRepo<Token>();
        _mockUnitOfWork = Mocker.GetMockUnitOfWork(_mockTokenRepo);
        _mockConfig = new();
        SetupMockConfig();
    }

    [Fact]
    public async Task Test_CreateAccessTokenAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);
        
        var user = new AppUser()
        {
            Id = "abcd123q4",
            Email = "abc@abc.com"
        };
        
        // ACT
        var data = await tokenService.CreateAccessTokenAsync(user);
        
        // ASSERT
        _mockUserRepo.Verify(ur => ur.GetRolesAsync(It.IsAny<AppUser>()), onceTime);
        Assert.NotEqual(0, data.Length);
    }

    [Fact]
    public async Task Test_CreateRefreshToken()
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
        var onceTime = Times.Once;
        
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
        _mockUserRepo.Verify(ur => ur.GetRolesAsync(It.IsAny<AppUser>()), onceTime);
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
        _mockTokenRepo.Verify(r => r.Add(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().Add(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), onceTime);
    }

    [Fact]
    public async Task Test_ValidateTokenAsync_Valid()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var expire = int.Parse(_mockConfig.Object["Jwt:RefreshTokenExpirationTime"]);
        var refreshExpire = DateTime.Now.AddDays(expire);
        var tokenNotExpire = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockTokenRepo
            .Setup(r => r.GetAsync(It.IsAny<ISpecification<Token>>()))
            .ReturnsAsync(tokenNotExpire);
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        var data = await tokenService.ValidateTokenAsync("");
        
        // ASSERT
        _mockTokenRepo.Verify(r => r.GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        Assert.Equal(tokenNotExpire, data);
    }

    [Fact]
    public async Task Test_ValidateTokenAsync_Token_Not_Found()
    {
        // ARRANGE
        var onceTime = Times.Once();
        _mockTokenRepo
            .Setup(r => r.GetAsync(It.IsAny<ISpecification<Token>>()))
            .ReturnsAsync((Token?)null);
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        var data = await tokenService.ValidateTokenAsync("");
        
        // ASSERT
        _mockTokenRepo.Verify(r => r.GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_ValidateTokenAsync_Token_Expire()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var refreshExpire = DateTime.Now.Subtract(TimeSpan.FromDays(1));
        var tokenExpire = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockTokenRepo
            .Setup(r => r.GetAsync(It.IsAny<ISpecification<Token>>()))
            .ReturnsAsync(tokenExpire);
        
        var tokenService = new TokenService(_mockUserRepo.Object, _mockConfig.Object, _mockUnitOfWork.Object);

        // ACT
        var data = await tokenService.ValidateTokenAsync("");
        
        // ASSERT
        _mockTokenRepo.Verify(r => r.GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
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
        _mockTokenRepo.Verify(r => r.Update(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().Update(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), onceTime);
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
        _mockTokenRepo.Verify(r => r.Delete(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().Delete(It.IsAny<Token>()), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), onceTime);
    }

    [Fact]
    public async Task Test_ValidateTokensAsync_Valid()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        var expire = int.Parse(_mockConfig.Object["Jwt:RefreshTokenExpirationTime"]);
        var refreshExpire = DateTime.Now.AddDays(expire);
        var tokenNotExpire = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockConfig
            .SetupGet(c => c[It.Is<string>(s => s == "Jwt:AccessTokenExpirationTime")])
            .Returns("0");
        
        _mockTokenRepo
            .Setup(r => r.GetAsync(It.IsAny<ISpecification<Token>>()))
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
        _mockTokenRepo.Verify(r => r.GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Test_ValidateTokensAsync_AccessToken_Not_Expire()
    {
        // ARRANGE
        var never = Times.Never();
        
        var expire = int.Parse(_mockConfig.Object["Jwt:RefreshTokenExpirationTime"]);
        var refreshExpire = DateTime.Now.AddDays(expire);
        var tokenNotExpire = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockTokenRepo
            .Setup(r => r.GetAsync(It.IsAny<ISpecification<Token>>()))
            .ReturnsAsync(tokenNotExpire);

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
        _mockTokenRepo.Verify(r => r.GetAsync(It.IsAny<ISpecification<Token>>()), never);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().GetAsync(It.IsAny<ISpecification<Token>>()), never);
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_ValidateTokensAsync_Token_NotFound()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        _mockConfig
            .SetupGet(c => c[It.Is<string>(s => s == "Jwt:AccessTokenExpirationTime")])
            .Returns("0");
        
        _mockTokenRepo
            .Setup(r => r.GetAsync(It.IsAny<ISpecification<Token>>()))
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
        _mockTokenRepo.Verify(r => r.GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_ValidationTokensAsync_RefreshToken_Expired()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        var refreshExpire = DateTime.Now.Subtract(TimeSpan.FromDays(1));
        var expiredToken = new Token()
        {
            RefreshExpiryDate = refreshExpire
        };
        
        _mockConfig
            .SetupGet(c => c[It.Is<string>(s => s == "Jwt:AccessTokenExpirationTime")])
            .Returns("0");
        
        _mockTokenRepo
            .Setup(r => r.GetAsync(It.IsAny<ISpecification<Token>>()))
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
        _mockTokenRepo.Verify(r => r.GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Token>().GetAsync(It.IsAny<ISpecification<Token>>()), onceTime);
        Assert.Null(data);
    }

    private void SetupMockConfig()
    {
        _mockConfig
            .SetupGet(c => c[It.Is<string>(s => s == "Jwt:AccessTokenExpirationTime")])
            .Returns("1");
        _mockConfig
            .SetupGet(c => c[It.Is<string>(s => s == "Jwt:RefreshTokenExpirationTime")])
            .Returns("90");
        _mockConfig
            .SetupGet(c => c[It.Is<string>(s => s == "Jwt:AccessTokenSecret")])
            .Returns("Mock token secret key");
    }
}