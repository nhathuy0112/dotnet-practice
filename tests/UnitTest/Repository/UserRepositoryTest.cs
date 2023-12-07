using Domain.Entities;
using Domain.QueryParams.User;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace UnitTest.Repository;

public class UserRepositoryTest
{
    private List<AppUser> _userData;
    private List<IdentityRole> _roleData;
    private Mock<UserManager<AppUser>> _mockUserManager;
    private Mock<AppDbContext> _mockContext;
    private Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private int _originalUserDataCount;
    private int _originalRoleDataCount;

    public UserRepositoryTest()
    {
        SetupUserData();
        SetupRoleData();
        SetupMockContext();
        SetupMockUserManager();
        SetupMockRoleManager();
    }

    [Fact]
    public async Task Test_RegisterAsync_Success()
    {
        // ARRANGE
        var once = Times.Once();
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        
        var newUser = new AppUser() { Email = "test@abc.com" };
        var role = Role.User;
        
        // ACT
        await userRepo.RegisterAsync(newUser, "", role);
        
        // ASSERT
        Assert.Equal(4, _userData.Count);
        Assert.Equal("test@abc.com", _userData[^1].Email);
    }

    [Fact]
    public async Task Test_RegisterAsync_Create_Success_AddRole_Fail()
    {
        // ARRANGE
        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        
        var newUser = new AppUser() { Email = "test@abc.com" };
        var role = Role.User;

        // ACT
        await userRepo.RegisterAsync(newUser, "", role);
        
        // ASSERT
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(_originalUserDataCount, _userData.Count);
    }

    [Fact]
    public async Task Test_RegisterAsync_Create_Fail_AddRole_Success()
    {
        // ARRANGE
        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        
        var newUser = new AppUser() { Email = "test@abc.com" };
        var role = Role.User;

        // ACT
        await userRepo.RegisterAsync(newUser, "", role);
        
        // ASSERT
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(_originalUserDataCount, _userData.Count);
    }
    
    [Fact]
    public async Task Test_RegisterAsync_Create_Fail_AddRole_Fail()
    {
        // ARRANGE
        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        
        var newUser = new AppUser() { Email = "test@abc.com" };
        var role = Role.User;

        // ACT
        await userRepo.RegisterAsync(newUser, "", role);
        
        // ASSERT
        Assert.Equal(_originalUserDataCount, _userData.Count);
    }

    [Fact]
    public async Task Test_GetByEmailAsync_Not_Null()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        string email = "user1@abc.com";
        
        // ACT
        var data = await userRepo.GetByEmailAsync(email);
        
        // ASSERT
        Assert.NotNull(data);
    }
    
    [Fact]
    public async Task Test_GetByEmailAsync_Null()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        string email = "user1111@abc.com";
        
        // ACT
        var data = await userRepo.GetByEmailAsync(email);
        
        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_LoginAsync_Success()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        string email = "user1@abc.com";
        
        // ACT
        var data = await userRepo.LoginAsync(email, "");
        
        // ASSERT
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Test_LoginAsync_Wrong_Password()
    {
        // ARRANGE
        _mockUserManager
            .Setup(m => m.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        
        string email = "user1@abc.com";

        // ACT
        var data = await userRepo.LoginAsync(email, "");
        
        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_LoginAsync_Wrong_Email()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        
        string email = "user11111@abc.com";

        // ACT
        var data = await userRepo.LoginAsync(email, "");
        
        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_LoginAsync_Wrong_Email_And_Password()
    {
        // ARRANGE
        _mockUserManager
            .Setup(m => m.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);
        
        string email = "user11111@abc.com";

        // ACT
        var data = await userRepo.LoginAsync(email, "");
        
        // ASSERT
        Assert.Null(data);
    }

    [Fact]
    public async Task Test_GetRolesAsync()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        var data = await userRepo.GetRolesAsync(new AppUser());
        
        // ASSERT
        Assert.NotNull(data);
        Assert.Equal(_originalRoleDataCount, data.Count);
    }

    [Fact]
    public async Task Test_GetUsersAsync_With_Filter()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        var queryPamrams = new UserQueryParams()
        {
            Email = "user",
            Sort = "email"
        };
        
        // ACT
        var data = await userRepo.GetUsersAsync(queryPamrams, "1");
        
        // ASSERT
        Assert.Equal(2, data.Count);
    }

    [Fact]
    public async Task Test_GetRoleAsync()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        var role = Role.User;
        
        // ACT
        var data = await userRepo.GetRoleAsync(role);
        
        // ASSET
        Assert.NotNull(data);
    }

    [Fact]
    public async Task Test_CountUserAsync()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        var data = await userRepo.CountUsersAsync(new UserQueryParams(), "1");
        
        // ASSER
        Assert.Equal(2, data);
    }

    [Fact]
    public async Task Test_UpdateUserToRoleAdminAsync_Success()
    {
        // ARRANGE
        var once = Times.Once();
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        await userRepo.UpdateUserToRoleAdminAsync(new AppUser());
        
        // ASSERT
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()), once);
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.CommitAsync(It.IsAny<CancellationToken>()), once);
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task Test_UpdateUserToRoleAdmin_RemoveFromRole_Success_AddToRole_Fail()
    {
        // ARRANGE
        var once = Times.Once;
        
        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        await userRepo.UpdateUserToRoleAdminAsync(new AppUser());
        
        // ASSERT
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()), once);
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Test_UpdateUserToRoleAdmin_RemoveFromRole_Fail_AddToRole_Success()
    {
        // ARRANGE
        var once = Times.Once;
        
        _mockUserManager
            .Setup(m => m.RemoveFromRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        await userRepo.UpdateUserToRoleAdminAsync(new AppUser());
        
        // ASSERT
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()), once);
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Test_UpdateUserToRoleAdmin_RemoveFromRole_Fail_AddToRole_Fail()
    {
        // ARRANGE
        var once = Times.Once;

        _mockUserManager
            .Setup(m => m.RemoveFromRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());
        
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        await userRepo.UpdateUserToRoleAdminAsync(new AppUser());
        
        // ASSERT
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()), once);
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _mockContext.Verify(c => c.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Test_HashPassword()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        var data = userRepo.HashPassword(new AppUser(), "");
        
        // ASSERT
        Assert.NotEqual(0, data.Length);
    }

    [Fact]
    public async Task Test_UpdateUserAsync()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        await userRepo.UpdateUserAsync(new AppUser());
        
        // ASSERT
        _mockUserManager.Verify(m => m.UpdateAsync(It.IsAny<AppUser>()), Times.Once);
    }

    [Fact]
    public async Task Test_DeleteUserAsync()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        await userRepo.DeleteUserAsync(new AppUser());
        
        // ASSERT
        _mockUserManager.Verify(m => m.DeleteAsync(It.IsAny<AppUser>()), Times.Once);
    }

    [Fact]
    public async Task Test_GetByIdAsync()
    {
        // ARRANGE
        var userRepo = new UserRepository(_mockContext.Object, _mockUserManager.Object, _mockRoleManager.Object);

        // ACT
        var data = await userRepo.GetByIdAsync("1");
        
        // ASSERT
        Assert.NotNull(data);
        Assert.Equal("User 1", data.UserName);
    }

    private void SetupUserData()
    {
        _userData = new List<AppUser>()
        {
            new()
            {
                Id = "1",
                UserName = "User 1",
                Email = "user1@abc.com",
                UserRoles = new List<IdentityUserRole<string>>()
                {
                    new()
                    {
                        RoleId = "1",
                        UserId = "1"
                    }
                }
            },
            new()
            {
                Id = "2",
                UserName = "User 2",
                Email = "Alpha-user2@abc.com",
                UserRoles = new List<IdentityUserRole<string>>()
                {
                    new()
                    {
                        RoleId = "1",
                        UserId = "2"
                    }
                }
            },
            new()
            {
                Id = "3",
                UserName = "Admin",
                Email = "Admin@abc.com",
                UserRoles = new List<IdentityUserRole<string>>()
                {
                    new()
                    {
                        RoleId = "2",
                        UserId = "3"
                    }
                }
            }
        };
        _originalUserDataCount = _userData.Count;
    }

    private void SetupRoleData()
    {
        _roleData = new List<IdentityRole>()
        {
            new()
            {
                Id = "1",
                Name = Role.User.ToString()
            },
            new()
            {
                Id = "2",
                Name = Role.Admin.ToString()
            }
        };
        _originalRoleDataCount = _roleData.Count;
    }

    private void SetupMockContext()
    {
        var option = new DbContextOptionsBuilder()
            .Options;
        _mockContext = new(option);

        var mockDbFacade = new Mock<DatabaseFacade>(_mockContext.Object);

        mockDbFacade
            .Setup(m => m.CreateExecutionStrategy())
            .Returns(() => new Mock<ExecutionStrategy>().Object);

        var mockTransaction = new Mock<IDbContextTransaction>();
        _mockContext
            .SetupGet(m => m.Database)
            .Returns(mockDbFacade.Object);

        _mockContext
            .Setup(m => m.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTransaction.Object);

        var dataBeforeRollBack = new List<AppUser>();
        dataBeforeRollBack.AddRange(_userData);
        _mockContext
            .Setup(m => m.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()).Result.RollbackAsync(It.IsAny<CancellationToken>()))
            .Callback(() => _userData = dataBeforeRollBack);
    }

    private void SetupMockUserManager()
    {
        var mockUserStore = new Mock<IUserStore<AppUser>>();
        var mockPasswordHasher = new Mock<IPasswordHasher<AppUser>>();

        mockPasswordHasher
            .Setup(m => m.HashPassword(It.IsAny<AppUser>(), It.IsAny<string>()))
            .Returns("Password");
        
        _mockUserManager = new(mockUserStore.Object, null, mockPasswordHasher.Object, null, null, null, null, null, null);

        _mockUserManager
            .Setup(m => m.Users)
            .Returns(_userData.AsQueryable);

        _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback((AppUser user, string password) => _userData.Add(user));

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((string email) => _userData.SingleOrDefault(u => u.Email == email));

        _mockUserManager
            .Setup(m => m.CheckPasswordAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockUserManager
            .Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(_roleData.Select(r => r.Name).ToList());

        _mockUserManager
            .Setup(m => m.RemoveFromRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.UpdateAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);
        
        _mockUserManager
            .Setup(m => m.DeleteAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager
            .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => _userData.SingleOrDefault(u => u.Id == id));
    }

    private void SetupMockRoleManager()
    {
        var mockRoleStore = new Mock<IRoleStore<IdentityRole>>();
        _mockRoleManager = new(mockRoleStore.Object, null, null, null, null);

        _mockRoleManager
            .Setup(m => m.Roles)
            .Returns(_roleData.AsQueryable);
    }
}