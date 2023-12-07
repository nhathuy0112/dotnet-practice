using Application.Dto.Auth;
using Application.Dto.User;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Specification;
using Domain.Specification.User;
using Moq;
using UnitTest.Service.Setup;

namespace UnitTest.Service;

public class UserTest
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IMapper> _mockMapper;

    public UserTest()
    {
        _mockRepo = UserMocker.GetMockRepo();
        _mockTokenService = UserMocker.GetMockTokenService();
        _mockMapper = new();
        SetupMockMapper();
    }

    [Fact]
    public async Task Test_RegisterAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();

        _mockRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?) null);

       var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);
        
        // ACT
        var result = await userService.RegisterAsync(new RegisterRequest() {Email = "", Password = ""});
        
        // ASSERT
        _mockRepo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), onceTime);
        _mockRepo.Verify(r => r.RegisterAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<Role>()), onceTime);
    }

    [Fact]
    public async Task Test_RegisterAsync_Email_Existed()
    {
        // ARRANGE
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);
        
        // ACT
        var act = async () => await userService.RegisterAsync(new RegisterRequest());

        // ASSERT
        var exception = await Assert.ThrowsAsync<UserException>(act);
        Assert.Equal("Email existed", exception.Message);
    }

    [Fact]
    public async Task Test_RegisterAsync_Fail()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?) null);
        
        _mockRepo
            .Setup(r => r.RegisterAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<Role>()))
            .ReturnsAsync(false);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.RegisterAsync(new RegisterRequest());

        // ASSERT
        var exception = await Assert.ThrowsAsync<UserException>(act);
        Assert.Equal("Cannot register", exception.Message);
    }

    [Fact]
    public async Task Test_LoginAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        await userService.LoginAsync(new LoginRequest());
        
        //ASSERT
        _mockRepo.Verify(r => r.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), onceTime);
        _mockRepo.Verify(r => r.GetRolesAsync(It.IsAny<AppUser>()), onceTime);
        _mockTokenService.Verify(t => t.CreateTokensAsync(It.IsAny<AppUser>()), onceTime);
        _mockTokenService.Verify(t => t.SaveTokenAsync(It.IsAny<Token>()), onceTime);
    }

    [Fact]
    public async Task Test_LoginAsync_Fail()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.LoginAsync(new LoginRequest());
        
        // ASSERT
        var exception = await Assert.ThrowsAsync<UserException>(act);
        Assert.Equal("Wrong email or password", exception.Message);
    }

    [Fact]
    public async Task Test_RefreshAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);
        
        // ACT
        await userService.RefreshAsync(new RefreshRequest());
        
        // ASSERT
        _mockTokenService.Verify(t => t.ValidateTokensAsync(It.IsAny<string>(), It.IsAny<string>()), onceTime);
        _mockTokenService.Verify(t => t.CreateAccessTokenAsync(It.IsAny<AppUser>()), onceTime);
        _mockTokenService.Verify(t => t.UpdateTokenAsync(It.IsAny<Token>()), onceTime);
    }

    [Fact]
    public async Task Test_RefreshAsync_Fail()
    {
        // ARRANGE
        _mockTokenService
            .Setup(t => t.ValidateTokensAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((Token?)null);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.RefreshAsync(new RefreshRequest());
        
        // ASSERT
        var exception = await Assert.ThrowsAsync<UserException>(act);
        Assert.Equal("Tokens are not valid", exception.Message);
    }

    [Fact]
    public async Task Test_Logout_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        await userService.LogoutAsync("");
        
        // ASSERT
        _mockTokenService.Verify(t => t.ValidateTokenAsync(It.IsAny<string>()), onceTime);
        _mockTokenService.Verify(t => t.DeleteTokenAsync(It.IsAny<Token>()), onceTime);
    }

    [Fact]
    public async Task Test_Logout_Fail()
    {
        // ARRANGE
        _mockTokenService
            .Setup(t => t.ValidateTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((Token?)null);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.LogoutAsync("");
        
        //ASSERT
        var exception = await Assert.ThrowsAsync<UserException>(act);
        Assert.Equal("Token is not valid", exception.Message);
    }

    [Fact]
    public async Task Test_GetUsersAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);
        
        // ACT
        await userService.GetUsersAsync(new UserRequestParams());
        
        // ASSERT
        _mockRepo.Verify(r => r.GetRoleAsync(It.IsAny<Role>()), onceTime);
        _mockRepo.Verify(r => r.CountUsersAsync(It.IsAny<ISpecification<AppUser>>()), onceTime);
        _mockRepo.Verify(r => r.GetUsersAsync(It.IsAny<ISpecification<AppUser>>()), onceTime);
        _mockMapper.Verify(m => m.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserResponse>>(It.IsAny<IReadOnlyList<AppUser>>()), onceTime);
    }

    [Fact]
    public async Task Test_UpdateRoleAdminAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();

        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        await userService.UpdateUserToRoleAdminAsync("");
        
        // ASSERT
        _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<string>()), onceTime);
        _mockRepo.Verify(r => r.GetRolesAsync(It.IsAny<AppUser>()), onceTime);
        _mockRepo.Verify(r => r.UpdateUserToRoleAdminAsync(It.IsAny<AppUser>()), onceTime);
    }
    
    [Fact]
    public async Task Test_UpdateRoleAdminAsync_Not_UserRole()
    {
        // ARRANGE
        var onceTime = Times.Once();

        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.UpdateUserToRoleAdminAsync("");
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_UpdateUserToRoleAdminAsync_User_Not_Exist()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.UpdateUserToRoleAdminAsync("");
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_UpdateUserToRoleAdminAsync_Fail()
    {
        // ARRANGE
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(r => r.UpdateUserToRoleAdminAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(false);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.UpdateUserToRoleAdminAsync("");
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    

    [Fact]
    public async Task Test_UpdateUserAsync_Different_Email_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        await userService.UpdateUserAsync("", new UserRequest(){Email = "a"});
        
        // ASSERT
        _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<string>()), onceTime);
        _mockRepo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), onceTime);
        _mockRepo.Verify(r => r.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>()), onceTime);
        _mockRepo.Verify(r => r.UpdateUserAsync(It.IsAny<AppUser>()), onceTime);
        _mockMapper.Verify(m => m.Map<UserResponse>(It.IsAny<AppUser>()), onceTime);
    }
    
    [Fact]
    public async Task Test_UpdateUserAsync_Same_Email_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        await userService.UpdateUserAsync("", new UserRequest());
        
        // ASSERT
        _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<string>()), onceTime);
        _mockRepo.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        _mockRepo.Verify(r => r.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>()), onceTime);
        _mockRepo.Verify(r => r.UpdateUserAsync(It.IsAny<AppUser>()), onceTime);
        _mockMapper.Verify(m => m.Map<UserResponse>(It.IsAny<AppUser>()), onceTime);
    }

    [Fact]
    public async Task Test_UpdateUserAsync_User_Not_Exist()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.UpdateUserAsync("", new UserRequest());
        
        // ASSERT
        var exception = await Assert.ThrowsAsync<UserException>(act);
        Assert.Equal("User does not exist", exception.Message);
    }
    
    [Fact]
    public async Task Test_UpdateUserAsync_Not_UserRole()
    {
        // ARRANGE
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.UpdateUserAsync("", new UserRequest());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }


    [Fact]
    public async Task Test_UpdateUserAsync_Email_Existed()
    {
        // ARRANGE
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.UpdateUserAsync("", new UserRequest() { Email = "" });
        
        // ASSERT
        var exception = await Assert.ThrowsAsync<UserException>(act);
        Assert.Equal("Email existed", exception.Message);
    }

    [Fact]
    public async Task Test_UpdateUserAsync_DifferentEmail()
    {
        // ARRANGE
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        _mockRepo
            .Setup(r => r.UpdateUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(false);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.UpdateUserAsync("", new UserRequest(){Email = "a"});
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_UpdateUserAsync_Same_Email()
    {
        // ARRANGE
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(r => r.UpdateUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(false);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.UpdateUserAsync("", new UserRequest());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_DeleteUserAsync_Success()
    {
        // ARRANGE
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        await userService.DeleteUserAsync("");
        
        // ASSERT
        _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<string>()));
        _mockRepo.Verify(r => r.DeleteUserAsync(It.IsAny<AppUser>()));
    }
    
    [Fact]
    public async Task Test_DeleteUserAsync_Not_UserRole()
    {
        // ARRANGE
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.DeleteUserAsync("");
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_DeleteUserAsync_User_Not_Exist()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.DeleteUserAsync("");
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_DeleteUserAsync_Fail()
    {
        // ARRANGE
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(r => r.DeleteUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(false);
        
        var userService = new UserService(_mockRepo.Object, _mockTokenService.Object, _mockMapper.Object);

        // ACT
        var act = async () => await userService.DeleteUserAsync("");
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    private void SetupMockMapper()
    {
        _mockMapper
            .Setup(m => m.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserResponse>>(It.IsAny<IReadOnlyList<AppUser>>()))
            .Returns(new List<UserResponse>());
        _mockMapper
            .Setup(m => m.Map<AppUser, UserResponse>(It.IsAny<AppUser>()))
            .Returns(new UserResponse()
            {
                Id = "",
                Email = ""
            });
    }
}