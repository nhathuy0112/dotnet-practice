using Application.Interfaces;
using Domain.Entities;
using Domain.Specification;
using Microsoft.AspNetCore.Identity;
using Moq;
using Range = Moq.Range;

namespace UnitTest.Service.Setup;

public class UserMocker
{
    public static Mock<IUserRepository> GetMockRepo()
    {
        var mockRepo = new Mock<IUserRepository>();
        SetUpMockRepo(mockRepo);
        return mockRepo;
    }

    public static Mock<ITokenService> GetMockTokenService()
    {
        var mockTokenService = new Mock<ITokenService>();
        SetUpMockTokenService(mockTokenService);
        return mockTokenService;
    }

    private static void SetUpMockRepo(Mock<IUserRepository> mockRepo)
    {
        var user = new AppUser();
        
        mockRepo
            .Setup(r => r.RegisterAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<Role>()))
            .ReturnsAsync(true);

        mockRepo
            .Setup(r => r.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AppUser());

        mockRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());

        mockRepo
            .Setup(r => r.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>());

        mockRepo
            .Setup(r => r.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>()))
            .Returns("");

        mockRepo
            .Setup(r => r.CountUsersAsync(It.IsAny<ISpecification<AppUser>>()))
            .ReturnsAsync(1);

        mockRepo
            .Setup(r => r.DeleteUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);

        mockRepo
            .Setup(r => r.GetRoleAsync(It.IsAny<Role>()))
            .ReturnsAsync(new IdentityRole());

        mockRepo
            .Setup(r => r.GetUsersAsync(It.IsAny<ISpecification<AppUser>>()))
            .ReturnsAsync(new List<AppUser>());

        mockRepo
            .Setup(r => r.UpdateUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);

        mockRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());

        mockRepo
            .Setup(r => r.UpdateUserToRoleAdminAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);
    }

    private static void SetUpMockTokenService(Mock<ITokenService> mockTokenService)
    {
        mockTokenService
            .Setup(t => t.CreateTokensAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new Token());

        mockTokenService
            .Setup(t => t.DeleteTokenAsync(It.IsAny<Token>()));

        mockTokenService
            .Setup(t => t.SaveTokenAsync(It.IsAny<Token>()));

        mockTokenService
            .Setup(t => t.DeleteTokenAsync(It.IsAny<Token>()));

        mockTokenService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new Token());

        mockTokenService
            .Setup(t => t.ValidateTokensAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new Token());

        mockTokenService
            .Setup(t => t.UpdateTokenAsync(It.IsAny<Token>()));

        mockTokenService
            .Setup(t => t.CreateRefreshToken())
            .Returns("");

        mockTokenService
            .Setup(t => t.CreateAccessTokenAsync(It.IsAny<AppUser>()))
            .ReturnsAsync("");
    }
}