using Application.Interfaces;
using Domain.Common;
using Domain.Specification;
using Moq;
using Range = Moq.Range;

namespace UnitTest.Service.Setup;

public class Mocker
{
    public static Mock<IRepository<T>> GetMockRepo<T>() where T : BaseEntity
    {
        var mockRepo = new Mock<IRepository<T>>();
        SetupMockRepo(mockRepo);
        return mockRepo;
    }

    public static Mock<IUnitOfWork> GetMockUnitOfWork<T>(Mock<IRepository<T>> mockRepo) where T : BaseEntity
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork
            .Setup(u => u.Repository<T>())
            .Returns(mockRepo.Object);
        return mockUnitOfWork;
    }

    private static void SetupMockRepo<T>(Mock<IRepository<T>> mockRepo) where T : BaseEntity
    {
        var type = typeof(T);
        var newObject = Activator.CreateInstance(type);
        
        mockRepo
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<T>());

        mockRepo
            .Setup(r => r.GetByIdAsync(It.IsInRange(1, Int32.MaxValue, Range.Inclusive)))
            .ReturnsAsync((T) newObject);

        mockRepo
            .Setup(r => r.GetListAsync(It.IsAny<ISpecification<T>>()))
            .ReturnsAsync(new List<T>());

        // mockRepo
        //     .Setup(r => r.Add(It.IsAny<T>()));
        //
        // mockRepo
        //     .Setup(r => r.Update(It.IsAny<T>()));
        //
        // mockRepo
        //     .Setup(r => r.Delete(It.IsAny<T>()));

        mockRepo
            .Setup(r => r.GetAsync(It.IsAny<ISpecification<T>>()))
            .ReturnsAsync((T) newObject);

        mockRepo
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<T>>()))
            .ReturnsAsync(1);
    }
}