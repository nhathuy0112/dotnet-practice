using API.Controllers;
using Application.Categories.Commands.AddCategory;
using Application.Categories.Commands.DeleteCategory;
using Application.Categories.Commands.UpdateCategory;
using Application.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controller;

public class CategoryTest
{
    private readonly Mock<IMediator> _mockMediator = new();

    [Fact]
    public async Task Test_GetCategories()
    {
        // ARRANGE
        var once = Times.Once();

        var controller = new CategoryController(_mockMediator.Object);
        
        // ACT
        var data = await controller.GetCategories();
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<GetAllCategoriesQuery>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_AddCategory()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new CategoryController(_mockMediator.Object);

        // ACT
        var data = await controller.AddCategory(new AddCategoryCommand());
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<AddCategoryCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateCategory_Success()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new CategoryController(_mockMediator.Object);

        // ACT
        var data = await controller.UpdateCategory(1, new UpdateCategoryCommand() {Id = 1});
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateCategoryCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateCategory_BadRequest()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new CategoryController(_mockMediator.Object);

        // ACT
        var data = await controller.UpdateCategory(1, new UpdateCategoryCommand() {Id = 2});
        
        // ASSERT
        Assert.IsType<BadRequestResult>(data);
    }
    
    [Fact]
    public async Task Test_DeleteCategory()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new CategoryController(_mockMediator.Object);

        // ACT
        var data = await controller.DeleteCategory(1);
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<DeleteCategoryCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
}