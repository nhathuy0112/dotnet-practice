using Application.Common.Interfaces;
using Application.Users.Queries.GetUsers;
using AutoMapper;
using Domain.Entities;
using Domain.QueryParams.User;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace UnitTest.Handler.Users;

public class QueryHandlersTest
{
    private readonly Mock<IUserRepository> _mockRepo = new();
    private readonly Mock<IMapper> _mockMapper = new();
    
    [Fact]
    public async Task Test_GetUsers()
    {
        // ARRANGE
        var onceTime = Times.Once();

        _mockRepo
            .Setup(x => x.GetRoleAsync(It.IsAny<Role>()))
            .ReturnsAsync(new IdentityRole() {Id = ""});

        var handler = new GetUsersQueryHandler(_mockMapper.Object, _mockRepo.Object);
        
        // ACT
        await handler.Handle(new GetUserQuery(new UserQueryParams()), new CancellationToken());
        
        // ASSERT
        _mockRepo.Verify(x => x.GetRoleAsync(It.IsAny<Role>()), onceTime);
        _mockRepo.Verify(x => x.CountUsersAsync(It.IsAny<UserQueryParams>(), It.IsAny<string>()), onceTime);
        _mockRepo.Verify(x => x.GetUsersAsync(It.IsAny<UserQueryParams>(), It.IsAny<string>()), onceTime);
        _mockMapper.Verify(x => x.Map<IReadOnlyList<AppUser>, IReadOnlyList<GetUsersResponse>>(It.IsAny<IReadOnlyList<AppUser>>()), onceTime);
    }
}