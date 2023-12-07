using System.Security.Claims;
using API.Controllers;
using Application.Products.Commands.AddProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProducts;
using Domain.QueryParams.Product;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controller;

public class ProductTest
{
    private readonly Mock<IMediator> _mockMediator = new();

    [Fact]
    public async Task Test_GetProducts()
    {
        // ARRANGE
        var once = Times.Once();

        var controller = new ProductController(_mockMediator.Object);
        
        // ACT
        var data = await controller.GetProducts(new ProductQueryParams());
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_GetProductById()
    {
        // ARRANGE
        var once = Times.Once();

        var controller = new ProductController(_mockMediator.Object);
        
        // ACT
        var data = await controller.GetProductById(1);
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }

    [Fact]
    public async Task Test_AddProduct()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new ProductController(_mockMediator.Object)
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal() }
            }
        };

        // ACT
        var data = await controller.AddProduct(new AddProductCommand());
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<AddProductCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateProduct_Success()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new ProductController(_mockMediator.Object);

        // ACT
        var data = await controller.UpdateProduct(1, new UpdateProductCommand() {Id = 1});
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
    
    [Fact]
    public async Task Test_UpdateProduct_BadRequest()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new ProductController(_mockMediator.Object);

        // ACT
        var data = await controller.UpdateProduct(1, new UpdateProductCommand() {Id = 2});
        
        // ASSERT
        Assert.IsType<BadRequestResult>(data);
    }
    
    [Fact]
    public async Task Test_DeleteProduct()
    {
        // ARRANGE
        var once = Times.Once();
        
        var controller = new ProductController(_mockMediator.Object);

        // ACT
        var data = await controller.DeleteProduct(1);
        
        // ASSERT
        _mockMediator.Verify(x => x.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()), once);
        Assert.IsType<OkObjectResult>(data);
    }
}