using API.Controllers;
using Application.Users.Commands.Login;
using Application.Users.Commands.Logout;
using Application.Users.Commands.Refresh;
using Application.Users.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controller;

public class UserTest
{
    private readonly Mock<IMediator> _mockMediator = new();

    [Fact]
    public async Task Test_Register()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new UserController(_mockMediator.Object);
        
        // ACT
        var data = await controller.Register(new RegisterCommand());
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_Login()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new UserController(_mockMediator.Object);
        
        // ACT
        var data = await controller.Login(new LoginCommand());
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_Logout()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new UserController(_mockMediator.Object);
        
        // ACT
        var data = await controller.Logout("");
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<LogoutCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_Refresh()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new UserController(_mockMediator.Object);
        
        // ACT
        var data = await controller.Refresh(new RefreshCommand());
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<RefreshCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
}