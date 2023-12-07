using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using UnitTest.Repository.Specification;

namespace UnitTest.Repository;

public class RepositoryBaseTest
{
    private Mock<AppDbContext> _mockDbContext;
    private List<Product> _data;
    
    public RepositoryBaseTest()
    {
        SetupData();
        SetupMockDbContext();
    }

    [Fact]
    public async Task Test_GetAllAsync()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        // ACT
        var data = await repo.GetAllAsync();

        // ASSERT
        Assert.NotNull(data);
        Assert.Equal(3, data.Count);
    }

    [Fact]
    public async Task Test_GetListAsync_Filter_By_Name()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var productByNameSpec = new TestProductSpecification.TestProductByNameSpec("n");
        
        // ACT
        var data = await repo.GetListAsync(productByNameSpec);

        // ASSERT
        Assert.NotNull(data);
        Assert.Equal(2, data.Count);
        Assert.Equal("Nike", data[0].Name);
    }

    [Fact]
    public async Task Test_GetListAsync_Order_ByName()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var productByNameSpec = new TestProductSpecification.TestProductOrderByNameSpec();
        
        // ACT
        var data = await repo.GetListAsync(productByNameSpec);

        // ASSERT
        Assert.NotNull(data);
        Assert.Equal(3, data.Count);
        Assert.Equal("Adidas", data[0].Name);
    }

    [Fact]
    public async Task Test_GetListAsync_Order_By_Id_Desc()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var productByNameSpec = new TestProductSpecification.TestProductOrderByIdDescSpec();
        
        // ACT
        var data = await repo.GetListAsync(productByNameSpec);

        // ASSERT
        Assert.NotNull(data);
        Assert.Equal(3, data.Count);
        Assert.Equal(3, data[0].Id);
    }

    [Fact]
    public async Task Test_GetListAsync_Include_Category()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var productByNameSpec = new TestProductSpecification.TestProductIncludeCategorySpec();
        
        // ACT
        var data = await repo.GetListAsync(productByNameSpec);

        // ASSERT
        Assert.NotNull(data);
        Assert.Equal(3, data.Count);
        Assert.Null(data[0].Category);
        Assert.NotNull(data[2].Category);
    }

    [Fact]
    public async Task Test_GetListAsync_Pagination()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);
        int skip = 1;
        int take = 2;

        var productByNameSpec = new TestProductSpecification.TestProductWithPaginationSpec(skip: skip, take: take);
        
        // ACT
        var data = await repo.GetListAsync(productByNameSpec);

        // ASSERT
        Assert.NotNull(data);
        Assert.Equal(2, data.Count);
        Assert.Equal(2, data[0].Id);
        Assert.Equal(3, data[1].Id);
    }

    [Fact]
    public async Task Test_GetByIdAsync_Not_Null()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);
        int id = 1;

        // ACT
        var data = await repo.GetByIdAsync(id);

        // ASSERT
        Assert.NotNull(data);
        Assert.Equal("Nike", data?.Name);
    }
    
    [Fact]
    public async Task Test_GetByIdAsync_Null()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);
        int id = 0;

        // ACT
        var data = await repo.GetByIdAsync(id);

        // ASSERT
        Assert.Null(data);
    }
    
    [Fact]
    public async Task Test_GetAsync_By_Name_Not_Null()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var productByNameSpec = new TestProductSpecification.TestProductByNameSpec("Nike");
        
        // ACT
        var data = await repo.GetAsync(productByNameSpec);

        // ASSERT
        Assert.NotNull(data);
        Assert.Equal("Nike", data?.Name);
    }
    
    [Fact]
    public async Task Test_GetAsync_By_Name_Null()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var productByNameSpec = new TestProductSpecification.TestProductByNameSpec("Neki");
        
        // ACT
        var data = await repo.GetAsync(productByNameSpec);

        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_CountAsync_All()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var productByNameSpec = new TestProductSpecification.TestProductForCountSpec();
        
        // ACT
        var data = await repo.CountAsync(productByNameSpec);

        // ASSERT
        Assert.Equal(3, data);
    }
    
    [Fact]
    public async Task Test_CountAsync_With_Filter()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var productByNameSpec = new TestProductSpecification.TestProductForCountSpec("Nike");
        
        // ACT
        var data = await repo.CountAsync(productByNameSpec);

        // ASSERT
        Assert.Equal(1, data);
    }
    
    [Fact]
    public void Test_Add()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var product = new Product()
        {
            Name = "Vans"
        };
        
        // ACT
        repo.Add(product);

        // ASSERT
        _mockDbContext.Verify(m => m.Set<Product>().Add(It.IsAny<Product>()), Times.Once);
        _mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
        Assert.Equal(4, _data.Count);
    }
    
    // [Fact]
    // public async Task Test_Update()
    // {
    //     // ARRANGE
    //     var repo = new RepositoryBase<Product>(_mockDbContext.Object);
    //
    //     var existedProduct = await repo.GetByIdAsync(1);
    //     
    //     // ACT
    //     existedProduct.Name = "Vans";
    //     repo.Update(existedProduct);
    //
    //     // ASSERT
    //     _mockDbContext.Verify(m => m.Set<Product>().Attach(It.IsAny<Product>()), Times.Once);
    //     _mockDbContext.Verify(m => m.Entry(It.IsAny<Product>()).State, Times.Once);
    // }
    
    [Fact]
    public async Task Test_Delete()
    {
        // ARRANGE
        var repo = new RepositoryBase<Product>(_mockDbContext.Object);

        var existedProduct = await repo.GetByIdAsync(1);
        
        // ACT
        repo.Delete(existedProduct);

        // ASSERT
        _mockDbContext.Verify(m => m.Set<Product>().Remove(It.IsAny<Product>()), Times.Once);
        Assert.Equal(2, _data.Count);
        Assert.Equal(2, _data[0].Id);
    }

    private void SetupData()
    {
        _data = new List<Product>()
        {
            new()
            {
                Id = 1,
                Name = "Nike",
                CreatedBy = "admin@store.com",
                CreatedDate = DateTime.Now,
                Price = 2,
                CategoryId = 1
            },
            new()
            {
                Id = 2,
                Name = "Adidas",
                CreatedBy = "admin@store.com",
                CreatedDate = DateTime.Now,
                Price = 2,
                CategoryId = 1
            },
            new()
            {
                Id = 3,
                Name = "New balance",
                CreatedBy = "admin@store.com",
                CreatedDate = DateTime.Now,
                Price = 3,
                CategoryId = 2,
                Category = new Category()
                {
                    Id = 2,
                    Name = "Shoe"
                }
            }
        };
    }

    private void SetupMockDbContext()
    {
        var options = new DbContextOptionsBuilder().Options;
        _mockDbContext = new(options);

        var data = _data.AsQueryable();
        var mockSet = new Mock<DbSet<Product>>();
        
        mockSet
            .As<IAsyncEnumerable<Product>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncQuerySetup.TestAsyncEnumerator<Product>(data.GetEnumerator()));
        
        mockSet
            .As<IQueryable<Product>>()
            .Setup(m => m.Expression)
            .Returns(data.Expression);
        
        mockSet
            .As<IQueryable<Product>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQuerySetup.TestAsyncQueryProvider<Product>(data.Provider));

        mockSet
            .As<IQueryable<Product>>()
            .Setup(m => m.ElementType)
            .Returns(data.ElementType);
        
        mockSet
            .As<IQueryable<Product>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => data.GetEnumerator());
        
        _mockDbContext
            .Setup(c => c.Set<Product>())
            .Returns(data.BuildMockDbSet().Object);

        _mockDbContext
            .Setup(c => c.Set<Product>().Add(It.IsAny<Product>()))
            .Callback((Product product) => _data.Add(product));

        _mockDbContext
            .Setup(c => c.Set<Product>().Remove(It.IsAny<Product>()))
            .Callback((Product product) => _data.Remove(product));
    }
}