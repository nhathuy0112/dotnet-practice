using Application.Categories.Queries.GetAllCategories;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace UnitTest.Handler.Categories;

public class QueryHandlersTest
{
    private readonly Mock<ICategoryRepository> _mockRepo = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IMapper> _mockMapper = new();

    public QueryHandlersTest()
    {
        SetupMockUnitOfWork();
    }

    [Fact]
    public async Task Test_GetAllCategories()
    {
        // ARRANGE    
        var once = Times.Once();
        
        var handler = new GetAllCategoriesQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);

        // ACT
        await handler.Handle(new GetAllCategoriesQuery(), new CancellationToken());
        
        // ARRANGE
        _mockRepo.Verify(x => x.GetAllCategoriesAsync(), once);
        _mockMapper.Verify(x => x.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryResponse>>(It.IsAny<IReadOnlyList<Category>>()), once);
    }

    private void SetupMockUnitOfWork()
    {
        _mockUnitOfWork
            .Setup(x => x.CategoryRepository)
            .Returns(_mockRepo.Object);
    }
}