using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTest.Repository;

public class UnitOfWorkTest
{
    private Mock<AppDbContext> _mockContext;

    public UnitOfWorkTest()
    {
        SetupMockContext();
    }

    [Fact]
    public void Test_ProductRepository()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        var data = unitOfWork.ProductRepository;
        
        // ASSET
        Assert.Equal(typeof(ProductRepository), data.GetType());
    }
    
    [Fact]
    public void Test_Same_ProductRepository()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        var firstRepo = unitOfWork.ProductRepository;
        var secondRepo = unitOfWork.ProductRepository;
        
        // ASSET
        Assert.Same(firstRepo, secondRepo);
    }
    
    [Fact]
    public void Test_CategoryRepository()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        var data = unitOfWork.CategoryRepository;
        
        // ASSET
        Assert.Equal(typeof(CategoryRepository), data.GetType());
    }
    
    [Fact]
    public void Test_Same_CategoryRepository()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        var firstRepo = unitOfWork.CategoryRepository;
        var secondRepo = unitOfWork.CategoryRepository;
        
        // ASSET
        Assert.Same(firstRepo, secondRepo);
    }
    
    [Fact]
    public void Test_TokenRepository()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        var data = unitOfWork.TokenRepository;
        
        // ASSET
        Assert.Equal(typeof(TokenRepository), data.GetType());
    }
    
    [Fact]
    public void Test_Same_TokenRepository()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        var firstRepo = unitOfWork.TokenRepository;
        var secondRepo = unitOfWork.TokenRepository;
        
        // ASSET
        Assert.Same(firstRepo, secondRepo);
    }

    [Fact]
    public async Task Test_CompleteAsync()
    {
        // ARRANGE
        var once = Times.Once();
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        await unitOfWork.CompleteAsync();
        
        // ASSET
        _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), once);
    }

    [Fact]
    public void Test_Dispose()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);

        // ACT
        unitOfWork.Dispose();
        
        // ASSERT
        _mockContext.Verify(m => m.Dispose());
    }

    [Fact]
    public void Test_Dipose_Twice()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);

        // ACT
        unitOfWork.Dispose();
        unitOfWork.Dispose();
        
        // ASSERT
        _mockContext.Verify(m => m.Dispose(), Times.Once);
    }

    private void SetupMockContext()
    {
        var option = new DbContextOptionsBuilder().Options;
        _mockContext = new(option);
    }
}