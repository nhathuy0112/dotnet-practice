using API.Controllers;
using Application.Dto.Auth;
using Application.Dto.User;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Specification.User;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controller;

public class UserTest
{
    private readonly Mock<IUserService> _mockUserService;

    public UserTest()
    {
        _mockUserService = new();
        SetupMockUserService();
    }

    [Fact]
    public async Task Test_Register()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new UserController(_mockUserService.Object);
        
        // ACT
        var data = await controller.Register(new RegisterRequest());
        
        // ASSERT
        _mockUserService.Verify(s => s.RegisterAsync(It.IsAny<RegisterRequest>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_Login()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new UserController(_mockUserService.Object);
        
        // ACT
        var data = await controller.Login(new LoginRequest());
        
        // ASSERT
        _mockUserService.Verify(s => s.LoginAsync(It.IsAny<LoginRequest>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_Logout()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new UserController(_mockUserService.Object);
        
        // ACT
        var data = await controller.Logout("");
        
        // ASSERT
        _mockUserService.Verify(s => s.LogoutAsync(It.IsAny<string>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_Refresh()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new UserController(_mockUserService.Object);
        
        // ACT
        var data = await controller.Refresh(new RefreshRequest());
        
        // ASSERT
        _mockUserService.Verify(s => s.RefreshAsync(It.IsAny<RefreshRequest>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    private void SetupMockUserService()
    {
        _mockUserService
            .Setup(s => s.RegisterAsync(It.IsAny<RegisterRequest>()))
            .ReturnsAsync(true);

        _mockUserService
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(new LoginResponse());

        _mockUserService
            .Setup(s => s.RefreshAsync(It.IsAny<RefreshRequest>()))
            .ReturnsAsync(new RefreshResponse());

        _mockUserService
            .Setup(s => s.LogoutAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        
    }
}