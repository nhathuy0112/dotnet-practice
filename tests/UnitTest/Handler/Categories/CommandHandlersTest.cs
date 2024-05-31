using Application.Categories.Commands.AddCategory;
using Application.Categories.Commands.DeleteCategory;
using Application.Categories.Commands.UpdateCategory;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using Range = Moq.Range;

namespace UnitTest.Handler.Categories;

public class CommandHandlersTest
{
    private readonly Mock<ICategoryRepository> _mockRepo = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IMapper> _mockMapper = new();

    public CommandHandlersTest()
    {
        SetupMockUnitOfWork();
    }

    [Fact]
    public async Task Test_AddCategory()
    {
        // ARRANGE
        var once = Times.Once();
        var handler = new AddCategoryCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        await handler.Handle(new AddCategoryCommand(), new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.AddAsync(It.IsAny<Category>()), once);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), once);
        _mockMapper.Verify(x => x.Map<AddCategoryResponse>(It.IsAny<Category>()), once);
    }

    [Fact]
    public async Task Test_UpdateCategory_Success()
    {
        // ARRANGE
        var once = Times.Once();

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(new Category());
        
        var handler = new UpdateCategoryCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        await handler.Handle(new UpdateCategoryCommand() {Id = 1}, new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), once);
        _mockRepo.Verify(x => x.Update(It.IsAny<Category>()), once);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), once);
        _mockMapper.Verify(x => x.Map<UpdateCategoryResponse>(It.IsAny<Category>()), once);
    }
    
    [Fact]
    public async Task Test_UpdateCategory_NotFound()
    {
        // ARRANGE
        var handler = new UpdateCategoryCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        
        // ACT
        var act = async () => await handler.Handle(new UpdateCategoryCommand() {Id = 1}, new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<CategoryException>(act);
    }
    
    [Fact]
    public async Task Test_DeleteCategory_Success()
    {
        // ARRANGE
        var once = Times.Once();

        _mockRepo
            .Setup(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync(new Category());
        
        var handler = new DeleteCategoryCommandHandler(_mockUnitOfWork.Object);
        
        // ACT
        await handler.Handle(new DeleteCategoryCommand(1), new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)), once);
        _mockRepo.Verify(x => x.Delete(It.IsAny<Category>()), once);
        _mockUnitOfWork.Verify(x => x.CompleteAsync(), once);
    }
    
    [Fact]
    public async Task Test_DeleteCategory_NotFound()
    {
        // ARRANGE
        var handler = new DeleteCategoryCommandHandler(_mockUnitOfWork.Object);
        
        // ACT
        var act = async () => await handler.Handle(new DeleteCategoryCommand(1), new CancellationToken());
        
        // ASSERT
        await Assert.ThrowsAsync<CategoryException>(act);
    }

    private void SetupMockUnitOfWork()
    {
        _mockUnitOfWork
            .Setup(x => x.CategoryRepository)
            .Returns(_mockRepo.Object);
    }
}