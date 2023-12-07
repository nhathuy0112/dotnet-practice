using Application.Dto.Category;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using UnitTest.Service.Setup;
using Range = Moq.Range;

namespace UnitTest.Service;

public class CategoryTest
{
    private readonly Mock<IRepository<Category>> _mockRepo;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;

    public CategoryTest()
    {
        _mockRepo = Mocker.GetMockRepo<Category>();
        _mockUnitOfWork = Mocker.GetMockUnitOfWork(_mockRepo);
        _mockMapper = new();
        SetupMockMapper();
    }

    [Fact]
    public async Task Test_GetCategoriesAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();
        var categoryService = new CategoryService(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        var data = await categoryService.GetCategoriesAsync();
        
        // ASSERT
        _mockRepo.Verify(r => r.GetAllAsync(), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Category>(), onceTime);
        _mockMapper.Verify(m => m.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryResponse>>(It.IsAny<IReadOnlyList<Category>>()), onceTime);
    }

    [Fact]
    public async Task Test_AddCategoryAsync()
    {
        // ARRANGE
        var onceTime = Times.Once();
        var categoryService = new CategoryService(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        await categoryService.AddCategoryAsync(new CategoryRequest());
        
        // ASSERT
        _mockRepo.Verify(r => r.Add(It.IsAny<Category>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Category>(), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), onceTime);
        _mockMapper.Verify(m => m.Map<CategoryResponse>(It.IsAny<Category>()), onceTime);
    }

    [Fact]
    public async Task Test_UpdateCategoryAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();
        var categoryService = new CategoryService(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        await categoryService.UpdateCategoryAsync(1, new CategoryRequest());
        
        //ASSERT
        _mockRepo.Verify(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), onceTime);
        _mockRepo.Verify(r => r.Update(It.IsAny<Category>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Category>().GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Category>().Update(It.IsAny<Category>()), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync());
    }

    [Fact]
    public async Task Test_UpdateCategoryAsync_Category_Not_Exist()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync((Category?)null);
        
        var categoryService = new CategoryService(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        var act = async () => await categoryService.UpdateCategoryAsync(1, new CategoryRequest());
        
        // ASSERT
        var exception = await Assert.ThrowsAsync<CategoryException>(act);
        Assert.Equal("Cannot find category", exception.Message);
    }

    [Fact]
    public async Task Test_DeleteCategoryAsync_Success()
    {
        // ARRANGE
        var onceTime = Times.Once();
        var categoryService = new CategoryService(_mockUnitOfWork.Object, _mockMapper.Object);
        
        //ACT
        await categoryService.DeleteCategoryAsync(1);
        
        //ASSERT
        _mockRepo.Verify(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), onceTime);
        _mockRepo.Verify(r => r.Delete(It.IsAny<Category>()), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Category>().GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), onceTime);
        _mockUnitOfWork.Verify(u => u.Repository<Category>().Delete(It.IsAny<Category>()), onceTime);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), onceTime);
   }

    [Fact]
    public async Task Test_DeleteCategoryAsync_Category_Not_Exist()
    {
        // ARRANGE
        _mockRepo
            .Setup(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync((Category?)null);
        
        var categoryService = new CategoryService(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        var act = async () => await categoryService.DeleteCategoryAsync(1);
        
        // ASSERT
        var exception = await Assert.ThrowsAsync<CategoryException>(act);
        Assert.Equal("Cannot find category", exception.Message);
    }

    private void SetupMockMapper()
    {
        _mockMapper
            .Setup(x => x.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryResponse>>(It.IsAny<IReadOnlyList<Category>>()))
            .Returns(new List<CategoryResponse>());
        _mockMapper
            .Setup(x => x.Map<CategoryResponse>(It.IsAny<Category>()))
            .Returns(new CategoryResponse());
    }
}