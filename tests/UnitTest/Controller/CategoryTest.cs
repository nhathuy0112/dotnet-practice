using System.Net;
using API.Controllers;
using Application.Dto.Category;
using Application.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Range = Moq.Range;

namespace UnitTest.Controller;

public class CategoryTest
{
    private readonly Mock<ICategoryService> _mockCategoryService;

    public CategoryTest()
    {
        _mockCategoryService = new();
        SetupMockCategoryService();
    }

    [Fact]
    public async Task Test_GetCategories()
    {
        // ARRANGE
        var once = Times.Once();

        var controller = new CategoryController(_mockCategoryService.Object);
        
        // ACT
        var data = await controller.GetCategories();
        
        // ASSERT
        _mockCategoryService.Verify(s => s.GetCategoriesAsync(), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_AddCategory()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new CategoryController(_mockCategoryService.Object);

        // ACT
        var data = await controller.AddCategory(new CategoryRequest());
        
        // ASSERT
        _mockCategoryService.Verify(s => s.AddCategoryAsync(It.IsAny<CategoryRequest>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateCategory()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new CategoryController(_mockCategoryService.Object);

        // ACT
        var data = await controller.UpdateCategory(1, new CategoryRequest());
        
        // ASSERT
        _mockCategoryService.Verify(s => s.UpdateCategoryAsync(It.IsInRange(1,Int32.MaxValue, Range.Inclusive),It.IsAny<CategoryRequest>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_DeleteCategory()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new CategoryController(_mockCategoryService.Object);

        // ACT
        var data = await controller.DeleteCategory(1);
        
        // ASSERT
        _mockCategoryService.Verify(s => s.DeleteCategoryAsync(It.IsInRange(1,Int32.MaxValue, Range.Inclusive)), once);
        Assert.IsType<OkObjectResult>(data);
    }

    private void SetupMockCategoryService()
    {
        _mockCategoryService
            .Setup(s => s.GetCategoriesAsync())
            .ReturnsAsync(new List<CategoryResponse>());

        _mockCategoryService
            .Setup(s => s.AddCategoryAsync(It.IsAny<CategoryRequest>()))
            .ReturnsAsync(new CategoryResponse());

        _mockCategoryService
            .Setup(s => s.UpdateCategoryAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive),
                It.IsAny<CategoryRequest>()))
            .ReturnsAsync(new CategoryResponse());

        _mockCategoryService
            .Setup(s => s.DeleteCategoryAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(true);
    }
}