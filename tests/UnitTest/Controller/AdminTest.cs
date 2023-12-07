using API.Controllers;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Commands.UpdateUserToAdmin;
using Application.Users.Queries.GetUsers;
using Domain.QueryParams.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controller;

public class AdminTest
{
    private readonly Mock<IMediator> _mockMediator = new();

    [Fact]
    public async Task Test_GetUsers()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new AdminController(_mockMediator.Object);
        
        // ACT
        var data = await controller.GetUsers(new UserQueryParams());
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<GetUserQuery>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_UpdateUser_Success()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new AdminController(_mockMediator.Object);
        
        // ACT
        var data = await controller.UpdateUser("", new UpdateUserCommand() {Id = ""});
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateUser_BadRequest()
    {
        // ARRANGE
        var controller = new AdminController(_mockMediator.Object);
        
        // ACT
        var data = await controller.UpdateUser("1", new UpdateUserCommand() {Id = "2"});
        
        // ASSERT
        Assert.IsType<BadRequestResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateRoleAdmin()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new AdminController(_mockMediator.Object);
        
        // ACT
        var data = await controller.UpdateRoleAdmin("");
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateUserToAdminCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_DeleteUser()
    {
        // ARRANGE
        var once = Times.Once();
        var controller = new AdminController(_mockMediator.Object);
        
        // ACT
        var data = await controller.DeleteUser("");
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

}