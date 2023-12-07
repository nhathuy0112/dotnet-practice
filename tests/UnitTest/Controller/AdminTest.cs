using API.Controllers;
using Application.Dto.User;
using Application.Helpers;
using Application.Interfaces;
using Domain.Specification.User;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controller;

public class AdminTest
{
    private readonly Mock<IUserService> _mockUserService;

    public AdminTest()
    {
        _mockUserService = new();
        SetupMockUserService();
    }
    
    [Fact]
    public async Task Test_GetUsers()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new AdminController(_mockUserService.Object);
        
        // ACT
        var data = await controller.GetUsers(new UserRequestParams());
        
        // ASSERT
        _mockUserService.Verify(s => s.GetUsersAsync(It.IsAny<UserRequestParams>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_UpdateUser()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new AdminController(_mockUserService.Object);
        
        // ACT
        var data = await controller.UpdateUser("",new UserRequest());
        
        // ASSERT
        _mockUserService.Verify(s => s.UpdateUserAsync(It.IsAny<string>(), It.IsAny<UserRequest>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateRoleAdmin()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new AdminController(_mockUserService.Object);
        
        // ACT
        var data = await controller.UpdateRoleAdmin("");
        
        // ASSERT
        _mockUserService.Verify(s => s.UpdateUserToRoleAdminAsync(It.IsAny<string>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_DeleteUser()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new AdminController(_mockUserService.Object);
        
        // ACT
        var data = await controller.DeleteUser("");
        
        // ASSERT
        _mockUserService.Verify(s => s.DeleteUserAsync(It.IsAny<string>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    private void SetupMockUserService()
    {
        _mockUserService
            .Setup(s => s.GetUsersAsync(It.IsAny<UserRequestParams>()))
            .ReturnsAsync(new PaginatedResponse<UserResponse>());

        _mockUserService
            .Setup(s => s.UpdateUserAsync(It.IsAny<string>(), It.IsAny<UserRequest>()))
            .ReturnsAsync(new UserResponse());

        _mockUserService
            .Setup(s => s.UpdateUserToRoleAdminAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockUserService
            .Setup(s => s.DeleteUserAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
    }
}