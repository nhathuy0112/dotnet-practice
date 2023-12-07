using Application.Common.Interfaces;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.Login;
using Application.Users.Commands.Logout;
using Application.Users.Commands.Refresh;
using Application.Users.Commands.Register;
using Application.Users.Commands.UpdateUser;
using Application.Users.Commands.UpdateUserToAdmin;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Moq;

namespace UnitTest.Handler.Users;

public class CommandHandlersTest
{
    private readonly Mock<IUserRepository> _mockRepo = new();
    private readonly Mock<ITokenService> _mockTokenService = new();
    private readonly Mock<IMapper> _mockMapper = new();
    
    [Fact]
    public async Task Test_Register_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();

        _mockRepo
            .Setup(r => r.RegisterAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<Role>()))
            .ReturnsAsync(true);

        var handler = new RegisterCommandHandler(_mockRepo.Object);
        
        // ACT
        await handler.Handle(new RegisterCommand() {Email = "", Password = ""}, new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), onceTime);
        _mockRepo.Verify(x => x.RegisterAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<Role>()), onceTime);
    }
    
    [Fact]
    public async Task Test_Register_Email_Existed()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());
        
        var handler = new RegisterCommandHandler(_mockRepo.Object);

        // ACT
        var act = async () =>
            await handler.Handle(new RegisterCommand() { Email = "", Password = "" }, new CancellationToken());

        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_Register_Fail()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?) null);
        
        _mockRepo
            .Setup(r => r.RegisterAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<Role>()))
            .ReturnsAsync(false);
        
        var handler = new RegisterCommandHandler(_mockRepo.Object);

        // ACT
        var act = async () =>
            await handler.Handle(new RegisterCommand() { Email = "", Password = "" }, new CancellationToken());

        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_Login_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();

        _mockRepo
            .Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AppUser());

        _mockTokenService
            .Setup(x => x.CreateTokensAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new Token());
        
        var handler = new LoginCommandHandler(_mockRepo.Object, _mockTokenService.Object);

        // ACT
        await handler.Handle(new LoginCommand() { Email = "", Password = "" }, new CancellationToken());
        
        //ASSERT
        _mockRepo.Verify(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()), onceTime);
        _mockRepo.Verify(x => x.GetRolesAsync(It.IsAny<AppUser>()), onceTime);
        _mockTokenService.Verify(x => x.CreateTokensAsync(It.IsAny<AppUser>()), onceTime);
        _mockTokenService.Verify(x => x.SaveTokenAsync(It.IsAny<Token>()), onceTime);
    }
    
    [Fact]
    public async Task Test_Login_Fail()
    {
        // ARRANGE
        var handler = new LoginCommandHandler(_mockRepo.Object, _mockTokenService.Object);

        // ACT
        var act = async () =>
            await handler.Handle(new LoginCommand() { Email = "", Password = "" }, new CancellationToken());
        
        // ASSERT
       await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_Refresh_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();

        _mockTokenService
            .Setup(x => x.ValidateTokensAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new Token());

        var handler = new RefreshCommandHandler(_mockTokenService.Object);
        
        // ACT
        await handler.Handle(new RefreshCommand() { AccessToken = "", RefreshToken = "" }, new CancellationToken());
        
        // ASSERT
        _mockTokenService.Verify(x => x.ValidateTokensAsync(It.IsAny<string>(), It.IsAny<string>()), onceTime);
        _mockTokenService.Verify(x => x.CreateAccessTokenAsync(It.IsAny<AppUser>()), onceTime);
        _mockTokenService.Verify(x => x.UpdateTokenAsync(It.IsAny<Token>()), onceTime);
    }
    
    [Fact]
    public async Task Test_Refresh_Fail()
    {
        // ARRANGE
        var handler = new RefreshCommandHandler(_mockTokenService.Object);

        // ACT
        var act = async () => await handler.Handle(new RefreshCommand() { AccessToken = "", RefreshToken = "" }, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_Logout_Success()
    {
        // ARRANGE
        _mockTokenService
            .Setup(x => x.ValidateTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new Token());
        
        var onceTime = Times.Once();

        var handler = new LogoutCommandHandler(_mockTokenService.Object);

        // ACT
        await handler.Handle(new LogoutCommand(""), new CancellationToken());
        
        // ASSERT
        _mockTokenService.Verify(x => x.ValidateTokenAsync(It.IsAny<string>()), onceTime);
        _mockTokenService.Verify(x => x.DeleteTokenAsync(It.IsAny<Token>()), onceTime);
    }
    
    [Fact]
    public async Task Test_Logout_Fail()
    {
        // ARRANGE
        var handler = new LogoutCommandHandler(_mockTokenService.Object);

        // ACT
        var act = async () => await handler.Handle(new LogoutCommand(""), new CancellationToken());
        
        //ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    
    [Fact]
    public async Task Test_UpdateUserToRoleAdmin_Success()
    {
        // ARRANGE
        var once = Times.Once();

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());

        _mockRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });

        _mockRepo
            .Setup(x => x.UpdateUserToRoleAdminAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);

        var handler = new UpdateUserToAdminCommandHandler(_mockRepo.Object);
        
        // ACT
        await handler.Handle(new UpdateUserToAdminCommand(""), new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetByIdAsync(It.IsAny<string>()), once);
        _mockRepo.Verify(x => x.GetRolesAsync(It.IsAny<AppUser>()), once);
        _mockRepo.Verify(x => x.UpdateUserToRoleAdminAsync(It.IsAny<AppUser>()), once);
    }
    
    [Fact]
    public async Task Test_UpdateUserToRole_Not_UserRole()
    {
        // ARRANGE
        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());

        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });
        
        var handler = new UpdateUserToAdminCommandHandler(_mockRepo.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateUserToAdminCommand(""), new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_UpdateUserToRoleAdmin_User_Not_Exist()
    {
        // ARRANGE
        var handler = new UpdateUserToAdminCommandHandler(_mockRepo.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateUserToAdminCommand(""), new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_UpdateUserToRoleAdmin_Fail()
    {
        // ARRANGE
        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());
        
        _mockRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(x => x.UpdateUserToRoleAdminAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(false);
        
        var handler = new UpdateUserToAdminCommandHandler(_mockRepo.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateUserToAdminCommand(""), new CancellationToken());

        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_UpdateUser_Change_Email_Success()
    {
        // ARRANGE
        var once = Times.Once();
        var existedUser = new AppUser()
        {
            Email = "a"
        };

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existedUser);
        
        _mockRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        _mockRepo
            .Setup(x => x.UpdateUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);
        
        var handler = new UpdateUserCommandHandler(_mockRepo.Object, _mockMapper.Object);

        // ACT
        await handler.Handle(new UpdateUserCommand() { Email = "abc" }, new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetByIdAsync(It.IsAny<string>()), once);
        _mockRepo.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), once);
        _mockRepo.Verify(x => x.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>()), once);
        _mockRepo.Verify(x => x.UpdateUserAsync(It.IsAny<AppUser>()), once);
        _mockMapper.Verify(x => x.Map<UpdateUserResponse>(It.IsAny<AppUser>()), once);
    }
    
    [Fact]
    public async Task Test_UpdateUser_Change_Email_Update_Fail()
    {
        // ARRANGE
        var once = Times.Once();
        var existedUser = new AppUser()
        {
            Email = "a"
        };

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existedUser);
        
        _mockRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        _mockRepo
            .Setup(x => x.UpdateUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(false);
        
        var handler = new UpdateUserCommandHandler(_mockRepo.Object, _mockMapper.Object);

        // ACT
        var act = async() => await handler.Handle(new UpdateUserCommand() { Email = "abc" }, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    
    [Fact]
    public async Task Test_UpdateUser_Not_Change_Email_Success()
    {
        // ARRANGE
        var once = Times.Once();
        var existedUser = new AppUser()
        {
            Email = "a"
        };

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existedUser);
        
        _mockRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        _mockRepo
            .Setup(x => x.UpdateUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);
        
        var handler = new UpdateUserCommandHandler(_mockRepo.Object, _mockMapper.Object);

        // ACT
        await handler.Handle(new UpdateUserCommand() { Email = "a" }, new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetByIdAsync(It.IsAny<string>()), once);
        _mockRepo.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        _mockRepo.Verify(x => x.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>()), once);
        _mockRepo.Verify(x => x.UpdateUserAsync(It.IsAny<AppUser>()), once);
        _mockMapper.Verify(x => x.Map<UpdateUserResponse>(It.IsAny<AppUser>()), once);
    }
    
    [Fact]
    public async Task Test_UpdateUser_Not_Change_Email_Update_Fail()
    {
        // ARRANGE
        var once = Times.Once();
        var existedUser = new AppUser()
        {
            Email = "a"
        };

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existedUser);
        
        _mockRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        _mockRepo
            .Setup(x => x.UpdateUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(false);
        
        var handler = new UpdateUserCommandHandler(_mockRepo.Object, _mockMapper.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateUserCommand() { Email = "a" }, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_UpdateUser_User_Not_Exist()
    {
        // ARRANGE
        var handler = new UpdateUserCommandHandler(_mockRepo.Object, _mockMapper.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateUserCommand() { Email = "a" }, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_UpdateUser_Not_User_Role()
    {
        // ARRANGE
        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());
        
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });
        
        var handler = new UpdateUserCommandHandler(_mockRepo.Object, _mockMapper.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateUserCommand() { Email = "a" }, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_UpdateUser_Email_Existed()
    {
        // ARRANGE
        var existedUser = new AppUser()
        {
            Email = "a"
        };
        
        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(existedUser);
        
        _mockRepo
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());
        
        _mockRepo
            .Setup(x => x.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        var handler = new UpdateUserCommandHandler(_mockRepo.Object, _mockMapper.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateUserCommand() { Email = "abc" }, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
    
    [Fact]
    public async Task Test_DeleteUser_Success()
    {
        // ARRANGE
        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());
        
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });

        _mockRepo
            .Setup(x => x.DeleteUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(true);
        
        var handler = new DeleteUserCommandHandler(_mockRepo.Object);

        // ACT
        await handler.Handle(new DeleteUserCommand(""), new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<string>()));
        _mockRepo.Verify(r => r.DeleteUserAsync(It.IsAny<AppUser>()));
    }
    
    [Fact]
    public async Task Test_DeleteUser_Not_UserRole()
    {
        // ARRANGE
        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());
        
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "Admin" });
        
        var handler = new DeleteUserCommandHandler(_mockRepo.Object);

        // ACT
        var act = async () => await handler.Handle(new DeleteUserCommand(""), new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_DeleteUser_User_Not_Exist()
    {
        // ARRANGE
        var handler = new DeleteUserCommandHandler(_mockRepo.Object);

        // ACT
        var act = async () => await handler.Handle(new DeleteUserCommand(""), new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }

    [Fact]
    public async Task Test_DeleteUser_Fail()
    {
        // ARRANGE
        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new AppUser());
        
        _mockRepo
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new List<string>() { "User" });
        
        _mockRepo
            .Setup(r => r.DeleteUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(false);
        
        var handler = new DeleteUserCommandHandler(_mockRepo.Object);

        // ACT
        var act = async () => await handler.Handle(new DeleteUserCommand(""), new CancellationToken());

        // ASSERT
        await Assert.ThrowsAsync<UserException>(act);
    }
}