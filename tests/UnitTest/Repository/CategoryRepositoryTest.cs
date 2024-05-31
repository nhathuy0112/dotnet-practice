using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace UnitTest.Repository;

public class CategoryRepositoryTest
{
    private Mock<AppDbContext> _mockContext;
    private List<Category> _data;

    public CategoryRepositoryTest()
    {
        SetupData();
        SetupContext();
    }

    [Fact]
    public async Task Test_GetAllCategoriesAsync()
    {
        // ARRANGE
        var categoryRepo = new CategoryRepository(_mockContext.Object);
        
        // ACT
        var data = await categoryRepo.GetAllAsync();
        
        // ASSERT
        Assert.Equal(_data.Count, data.Count);
    }

    [Fact]
    public async Task Test_GetCategoryByIdAsync()
    {
        // ARRANGE
        var categoryRepo = new CategoryRepository(_mockContext.Object);
        int id = 1;
        
        // ACT
        var data = await categoryRepo.GetByIdAsync(id);
        
        // ASSERT
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Test_TestAddCategoryAsync()
    {
        // ARRANGE
        var newCategory = new Category()
        {
            Id = 3
        };
        var categoryRepo = new CategoryRepository(_mockContext.Object);
        
        // ACT
        await categoryRepo.AddAsync(newCategory);
        
        // ASSERT
        Assert.Equal(newCategory.Id, _data[^1].Id);
    }

    [Fact]
    public void Test_DeleteCategoryAsync()
    {
        // ARRANGE
        var countBeforeDelete = _data.Count;
        var categoryToRemove = _data[0];
        var categoryRepo = new CategoryRepository(_mockContext.Object);

        // ACT
        categoryRepo.Delete(categoryToRemove);
        
        // ASSERT
        Assert.NotEqual(countBeforeDelete, _data.Count);
    }

    private void SetupData()
    {
        _data = new()
        {
            new()
            {
                Id = 1,
                Name = "Shoe"
            },
            new()
            {
                Id = 2,
                Name = "Car"
            }
        };
    }

    private void SetupContext()
    {
        var options = new DbContextOptionsBuilder().Options;
        _mockContext = new(options);
        
        _mockContext
            .Setup(x => x.Categories)
            .Returns(_data.AsQueryable().BuildMockDbSet().Object);

        _mockContext
            .Setup(x => x.Set<Category>())
            .Returns(_data.AsQueryable().BuildMockDbSet().Object);
        
        _mockContext
            .Setup(c => c.Set<Category>().Add(It.IsAny<Category>()))
            .Callback((Category category) => _data.Add(category));

        _mockContext
            .Setup(c => c.Set<Category>().Remove(It.IsAny<Category>()))
            .Callback((Category category) => _data.Remove(category));
    }
 }