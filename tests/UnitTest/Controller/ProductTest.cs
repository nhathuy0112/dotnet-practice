using System.Security.Claims;
using API.Controllers;
using Application.Dto.Product;
using Application.Helpers;
using Application.Interfaces;
using Domain.Specification.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Range = Moq.Range;

namespace UnitTest.Controller;

public class ProductTest
{
    private readonly Mock<IProductService> _mockProductService;

    public ProductTest()
    {
        _mockProductService = new();
        SetupMockProductService();
    }

    [Fact]
    public async Task Test_GetProducts()
    {
        // ARRANGE
        var once = Times.Once();

        var controller = new ProductController(_mockProductService.Object);
        
        // ACT
        var data = await controller.GetProducts(new ProductRequestParams());
        
        // ASSERT
        _mockProductService.Verify(s => s.GetProductsAsync(It.IsAny<ProductRequestParams>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_GetProductById()
    {
        // ARRANGE
        var once = Times.Once();

        var controller = new ProductController(_mockProductService.Object);
        
        // ACT
        var data = await controller.GetProductById(1);
        
        // ASSERT
        _mockProductService.Verify(s => s.GetProductByIdAsync(It.IsInRange(1,Int32.MaxValue, Range.Inclusive)), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_AddProduct()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new ProductController(_mockProductService.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() }
            }
        };

        // ACT
        var data = await controller.AddProduct(new ProductRequest());
        
        // ASSERT
        _mockProductService.Verify(s => s.AddProductAsync(It.IsAny<ProductRequest>(), It.IsAny<string>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateProduct()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new ProductController(_mockProductService.Object);

        // ACT
        var data = await controller.UpdateProduct(1, new ProductRequest());
        
        // ASSERT
        _mockProductService.Verify(s => s.UpdateProductAsync(It.IsInRange(1,Int32.MaxValue, Range.Inclusive),It.IsAny<ProductRequest>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_DeleteProduct()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new ProductController(_mockProductService.Object);

        // ACT
        var data = await controller.DeleteProduct(1);
        
        // ASSERT
        _mockProductService.Verify(s => s.DeleteProductAsync(It.IsInRange(1,Int32.MaxValue, Range.Inclusive)), once);
        Assert.IsType<OkObjectResult>(data);
    }

    private void SetupMockProductService()
    {
        _mockProductService
            .Setup(s => s.GetProductsAsync(It.IsAny<ProductRequestParams>()))
            .ReturnsAsync(new PaginatedResponse<ProductResponse>());

        _mockProductService
            .Setup(s => s.AddProductAsync(It.IsAny<ProductRequest>(), It.IsAny<string>()))
            .ReturnsAsync(new ProductResponse());
        
        _mockProductService
            .Setup(s => s.UpdateProductAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive),
                It.IsAny<ProductRequest>()))
            .ReturnsAsync(new ProductResponse());

        _mockProductService
            .Setup(s => s.DeleteProductAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(true);
    }
}