using Application.Interfaces;
using Domain.Entities;
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
    public void Test_Repository()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        var data = unitOfWork.Repository<Product>();
        
        // ASSET
        Assert.Equal(typeof(RepositoryBase<Product>), data.GetType());
    }

    [Fact]
    public void Test_Repository_Same_Repo()
    {
        // ARRANGE
        var unitOfWork = new UnitOfWork(_mockContext.Object);
        
        // ACT
        var firstData = unitOfWork.Repository<Product>();
        var secondData = unitOfWork.Repository<Product>();
        
        // ASSET
        Assert.Same(firstData, secondData);
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