using Application.Common.Interfaces;
using Application.Products.Commands.AddProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProduct;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using Range = Moq.Range;

namespace UnitTest.Handler.Products;

public class CommandHandlersTest
{
    private readonly Mock<IProductRepository> _mockRepo = new();
    private readonly Mock<ICategoryRepository> _mockCategoryRepo = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IMapper> _mockMapper = new();

    public CommandHandlersTest()
    {
        SetupMockUnitOfWork();
    }

    [Fact]
    public async Task Test_AddProduct_Success()
    {
        // ARRANGE
        var once = Times.Once();

        _mockCategoryRepo
            .Setup(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(new Category());

        var handler = new AddProductCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        await handler.Handle(new AddProductCommand() {CategoryId = 1}, new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.AddAsync(It.IsAny<Product>()), once);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), once);
    }
    
    [Fact]
    public async Task Test_AddProduct_Category_NotFound()
    {
        // ARRANGE
        var handler = new AddProductCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        var act = async() => await handler.Handle(new AddProductCommand(), new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<ProductException>(act);
    }

    [Fact]
    public async Task Test_UpdateProduct_Success()
    {
        // ARRANGE
        var once = Times.Once();
        
        _mockCategoryRepo
            .Setup(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(new Category());

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(new Product());
        
        var handler = new UpdateProductCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        await handler.Handle(new UpdateProductCommand() { Id = 1, CategoryId = 1 }, new CancellationToken());
        
        // ASSERT
        _mockCategoryRepo.Verify(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), once);
        _mockRepo.Verify(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), once);
        _mockRepo.Verify(x => x.Update(It.IsAny<Product>()));
        _mockUnitOfWork.Verify(x => x.CompleteAsync());
        _mockMapper.Verify(x => x.Map<UpdateProductResponse>(It.IsAny<Product>()));
    }
    
    [Fact]
    public async Task Test_UpdateProduct_Category_NotFound()
    {
        // ARRANGE
        var handler = new UpdateProductCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateProductCommand() { Id = 1, CategoryId = 1 }, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<ProductException>(act);
    }
    
    [Fact]
    public async Task Test_UpdateProduct_Product_NotFound()
    {
        // ARRANGE
        _mockCategoryRepo
            .Setup(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(new Category());
        
        var handler = new UpdateProductCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        var act = async () => await handler.Handle(new UpdateProductCommand() { Id = 1, CategoryId = 1 }, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<ProductException>(act);
    }

    [Fact]
    public async Task Test_DeleteProduct_Success()
    {
        // ARRANGE
        var once = Times.Once();
        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(new Product());

        var handler = new DeleteProductCommandHandler(_mockUnitOfWork.Object);
        
        //ACT
        await handler.Handle(new DeleteProductCommand(1), new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), once);
        _mockRepo.Verify(x => x.Delete(It.IsAny<Product>()), once);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), once);
    }
    
    [Fact]
    public async Task Test_DeleteProduct_NotFound()
    {
        // ARRANGE
        var handler = new DeleteProductCommandHandler(_mockUnitOfWork.Object);
        
        //ACT
        var act = async () => await handler.Handle(new DeleteProductCommand(1), new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<ProductException>(act);
    }
    
    private void SetupMockUnitOfWork()
    {
        _mockUnitOfWork
            .Setup(x => x.ProductRepository)
            .Returns(_mockRepo.Object);
        
        _mockUnitOfWork
            .Setup(x => x.CategoryRepository)
            .Returns(_mockCategoryRepo.Object);
    }
}