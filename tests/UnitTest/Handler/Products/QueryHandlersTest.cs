using Application.Common.Interfaces;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProducts;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.QueryParams.Product;
using Moq;
using Range = Moq.Range;

namespace UnitTest.Handler.Products;

public class QueryHandlersTest
{
    private readonly Mock<IProductRepository> _mockRepo = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IMapper> _mockMapper = new();

    public QueryHandlersTest()
    {
        SetupMockUnitOfWork();
    }
    
    [Fact]
    public async Task Test_GetProducts()
    {
        // ARRANGE
        var once = Times.Once();
        var handler = new GetProductsQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        await handler.Handle(new GetProductsQuery(new ProductQueryParams()), new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetAsync(It.IsAny<ProductQueryParams>()), once);
        _mockMapper.Verify(x => x.Map<IReadOnlyList<Product>, IReadOnlyList<GetProductsResponse>>(It.IsAny<IReadOnlyList<Product>>()), once);
    }

    [Fact]
    public async Task Test_GetProductById_Success()
    {
        // ARRANGE
        var once = Times.Once();

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(new Product());
        
        var handler = new GetProductByIdQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        await handler.Handle(new GetProductByIdQuery(1), new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), once);
        _mockMapper.Verify(x => x.Map<GetProductByIdResponse>(It.IsAny<Product>()));
    }
    
    [Fact]
    public async Task Test_GetProductById_NotFound()
    {
        // ARRANGE
        var handler = new GetProductByIdQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        var act = async () => await handler.Handle(new GetProductByIdQuery(1), new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<ProductException>(act);
    }
    
    private void SetupMockUnitOfWork()
    {
        _mockUnitOfWork
            .Setup(x => x.ProductRepository)
            .Returns(_mockRepo.Object);
    }
}