using Domain.Entities;
using Domain.QueryParams.Product;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace UnitTest.Repository;

public class ProductRepositoryTest
{
    private Mock<AppDbContext> _mockContext;
    private List<Product> _data;

    public ProductRepositoryTest()
    {
        SetupData();
        SetupMockContext();
    }

    [Fact]
    public async Task Test_GetProductsAsync()
    {
        // ARRANGE
        var queryParams = new ProductQueryParams();

        var productRepo = new ProductRepository(_mockContext.Object);
        
        // ACT
        var data = await productRepo.GetAsync(queryParams);
        
        // ASSET
        Assert.NotEmpty(data);
    }

    [Fact]
    public async Task Test_CountProductAsync()
    {
        // ARRANGE
        var queryParams = new ProductQueryParams();

        var productRepo = new ProductRepository(_mockContext.Object);
        
        // ACT
        var data = await productRepo.CountAsync(queryParams);
        
        // ASSERT
        Assert.NotEqual(0, data);
    }

    [Fact]
    public async Task Test_GetProductByIdAsync()
    {
        // ARRANGE
        int id = 1;
        
        var productRepo = new ProductRepository(_mockContext.Object);

        // ACT
        var data = await productRepo.GetByIdAsync(id);

        // ASSET
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Test_AddProductAsync()
    {
        // ARRANGE
        var newProduct = new Product()
        {
            Id = 3,
            Name = "Juice"
        };
        
        var productRepo = new ProductRepository(_mockContext.Object);

        // ACT
        await productRepo.AddAsync(newProduct);
        
        // ASSERT
        Assert.Equal(3, _data[^1].Id);
    }
    
    [Fact]
    public void Test_DeleteProductAsync()
    {
        // ARRANGE
        int countBeforeRemove = _data.Count;
        var productToRemove = _data[0];
        
        var productRepo = new ProductRepository(_mockContext.Object);

        // ACT
        productRepo.Delete(productToRemove);
        
        // ASSERT
        Assert.NotEqual(countBeforeRemove, _data.Count);
    }

    private void SetupData()
    {
        _data = new List<Product>()
        {
            new()
            {
                Id = 1,
                Name = "Iphone"
            },
            new()
            {
                Id = 2,
                Name = "Android"
            }
        };
    }
    
    private void SetupMockContext()
    {
        var options = new DbContextOptionsBuilder().Options;
        _mockContext = new(options);

        _mockContext
            .Setup(x => x.Products)
            .Returns(_data.AsQueryable().BuildMockDbSet().Object);

        _mockContext
            .Setup(x => x.Set<Product>())
            .Returns(_data.AsQueryable().BuildMockDbSet().Object);
        
        _mockContext
            .Setup(c => c.Set<Product>().Add(It.IsAny<Product>()))
            .Callback((Product product) => _data.Add(product));

        _mockContext
            .Setup(c => c.Set<Product>().Remove(It.IsAny<Product>()))
            .Callback((Product product) => _data.Remove(product));
    }
}