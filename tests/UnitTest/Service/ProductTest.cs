using Application.Dto.Product;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Specification;
using Domain.Specification.Product;
using Moq;
using UnitTest.Service.Setup;
using Range = Moq.Range;

namespace UnitTest.Service;

public class ProductTest
{
    private readonly Mock<IRepository<Product>> _mockProductRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    
    public ProductTest()
    {
        _mockProductRepo = Mocker.GetMockRepo<Product>();
        _mockUnitOfWork = Mocker.GetMockUnitOfWork(_mockProductRepo);
        _mockMapper = new();
        SetupMockMapper();
    }

    [Fact]
    public async Task Test_GetProductsAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        await productService.GetProductsAsync(new ProductRequestParams());
        
        // ASSERT
        _mockProductRepo.Verify(r => r.GetListAsync(It.IsAny<ISpecification<Product>>()), onceTime);
        _mockProductRepo.Verify(r => r.CountAsync(It.IsAny<ISpecification<Product>>()), onceTime);
        _mockMapper.Verify(m => m.Map<IReadOnlyList<Product>, IReadOnlyList<ProductResponse>>(It.IsAny<IReadOnlyList<Product>>()), onceTime);
    }

    [Fact]
    public async Task Test_GetProductByIdAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        await productService.GetProductByIdAsync(1);
        
        // ASSERT
        _mockProductRepo.Verify(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), onceTime);
        _mockMapper.Verify(m => m.Map<ProductResponse>(It.IsAny<Product>()), onceTime);
    }

    [Fact]
    public async Task Test_AddProductAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var mockCategoryRepo = Mocker.GetMockRepo<Category>();
        
        _mockUnitOfWork
            .Setup(m => m.Repository<Category>())
            .Returns(mockCategoryRepo.Object);

        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        await productService.AddProductAsync(new ProductRequest() {CategoryId = 1}, "admin@store.com");
        
        // ASSERT
        mockCategoryRepo.Verify(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), onceTime);
        _mockProductRepo.Verify(r => r.Add(It.IsAny<Product>()), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), onceTime);
        _mockMapper.Verify(m => m.Map<ProductResponse>(It.IsAny<Product>()), onceTime);
    }
    
    [Fact]
    public async Task Test_AddProductAsync_Category_Invalid()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var mockCategoryRepo = Mocker.GetMockRepo<Category>();

        mockCategoryRepo
            .Setup(m => m.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync((Category?)null);
        
        _mockUnitOfWork
            .Setup(m => m.Repository<Category>())
            .Returns(mockCategoryRepo.Object);

        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        var act = async () => await productService.AddProductAsync(new ProductRequest() {CategoryId = 1}, "admin@store.com");
        
        // ASSERT
        await Assert.ThrowsAsync<ProductException>(act);
    }

    [Fact]
    public async Task Test_UpdateProductAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();
        
        var mockCategoryRepo = Mocker.GetMockRepo<Category>();
        
        _mockUnitOfWork
            .Setup(m => m.Repository<Category>())
            .Returns(mockCategoryRepo.Object);

        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        await productService.UpdateProductAsync(1, new ProductRequest() {CategoryId = 1});
        
        // ASSERT
        _mockProductRepo.Verify(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), onceTime);
        _mockProductRepo.Verify(r => r.Update(It.IsAny<Product>()), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), onceTime);
        _mockMapper.Verify(m => m.Map<ProductResponse>(It.IsAny<Product>()), onceTime);
    }
    
    [Fact]
    public async Task Test_UpdateProductAsync_Category_Invalid()
    {
        // ARRANGE
        var mockCategoryRepo = Mocker.GetMockRepo<Category>();
        
        mockCategoryRepo
            .Setup(m => m.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync((Category?)null);
        
        _mockUnitOfWork
            .Setup(m => m.Repository<Category>())
            .Returns(mockCategoryRepo.Object);

        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        var act = async () => await productService.UpdateProductAsync(1, new ProductRequest() {CategoryId = 1});
        
        // ASSERT
        await Assert.ThrowsAsync<ProductException>(act);
    }

    [Fact]
    public async Task Test_UpdateProductAsync_Product_Not_Exist()
    {
        // ARRANGE
        _mockProductRepo
            .Setup(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync((Product?)null);
        
        var mockCategoryRepo = Mocker.GetMockRepo<Category>();
        
        _mockUnitOfWork
            .Setup(m => m.Repository<Category>())
            .Returns(mockCategoryRepo.Object);
        
        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        var act = async () => await productService.UpdateProductAsync(1, new ProductRequest() {CategoryId = 1});
        
        // ASSERT
        var exception = await Assert.ThrowsAsync<ProductException>(act);
        Assert.Equal("Cannot find product", exception.Message);
    }
    
    [Fact]
    public async Task Test_DeleteProductAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();

        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        await productService.DeleteProductAsync(1);
        
        // ASSERT
        _mockProductRepo.Verify(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), onceTime);
        _mockProductRepo.Verify(r => r.Delete(It.IsAny<Product>()), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), onceTime);
    }

    [Fact]
    public async Task Test_DeleteProductAsync_Product_Not_Exist()
    {
        // ARRANGE
        _mockProductRepo
            .Setup(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync((Product?)null);
        
        var productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        var act = async () => await productService.DeleteProductAsync(1);
        
        // ASSERT
        var exception = await Assert.ThrowsAsync<ProductException>(act);
        Assert.Equal("Cannot find product", exception.Message);
    }
    
    private void SetupMockMapper()
    {
        _mockMapper
            .Setup(x => x.Map<IReadOnlyList<Product>, IReadOnlyList<ProductResponse>>(It.IsAny<IReadOnlyList<Product>>()))
            .Returns(new List<ProductResponse>());
        _mockMapper
            .Setup(x => x.Map<ProductResponse>(It.IsAny<Product>()))
            .Returns(new ProductResponse());
    }
}