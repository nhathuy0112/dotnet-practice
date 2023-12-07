using System.Linq.Expressions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Specification;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRepository(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> RegisterAsync(AppUser user, string password, Role role)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var createResult = await _userManager.CreateAsync(user, password);
        var addRoleResult = await _userManager.AddToRoleAsync(user, role.ToString());
        
        if (createResult.Succeeded && addRoleResult.Succeeded)
        {
            await transaction.CommitAsync();
            return true;
        }
        
        await transaction.RollbackAsync();
        return false;
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user;
    }

    public async Task<AppUser?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        bool isPasswordCorrect = false;
        
        if (user != null)
        {
            isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
        }
        
        return isPasswordCorrect ? user : null;
    }

    public Task<IList<string>> GetRolesAsync(AppUser user)
    {
        return _userManager.GetRolesAsync(user);
    }

    public async Task<IReadOnlyList<AppUser>> GetUsersAsync(ISpecification<AppUser> specification)
    {
        return await Task.FromResult(ApplySpecification(specification).AsNoTracking().ToList());
    }

    public async Task<IdentityRole?> GetRoleAsync(Role role)
    {
        return await Task.FromResult(_roleManager.Roles.FirstOrDefault(r => r.Name == role.ToString()));
    }

    public async Task<int> CountUsersAsync(ISpecification<AppUser> specification)
    {
        return await Task.FromResult(ApplySpecification(specification).Count());
    }

    public async Task<AppUser?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<bool> UpdateUserToRoleAdminAsync(AppUser user)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var removeResult = await _userManager.RemoveFromRoleAsync(user, Role.User.ToString());
        var addResult = await _userManager.AddToRoleAsync(user, Role.Admin.ToString());

        if (removeResult.Succeeded && addResult.Succeeded)
        {
            await transaction.CommitAsync();
            return true;
        }
        
        await transaction.RollbackAsync();
        return false;
    }

    public string HashPassword(AppUser user, string password)
    {
        return _userManager.PasswordHasher.HashPassword(user, password);
    }

    public async Task<bool> UpdateUserAsync(AppUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(AppUser user)
    {
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
    
    private IQueryable<AppUser> ApplySpecification(ISpecification<AppUser> specification)
    {
        return SpecificationEvaluator<AppUser>.GetQuery(_userManager.Users, specification);
    }
}