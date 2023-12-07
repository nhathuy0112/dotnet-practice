using System.Linq.Expressions;
using Domain.Entities;
using Domain.Specification;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<bool> RegisterAsync(AppUser user, string password, Role role);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser?> LoginAsync(string email, string password);
    Task<IList<string>> GetRolesAsync(AppUser user);
    Task<IReadOnlyList<AppUser>> GetUsersAsync(ISpecification<AppUser> specification);
    Task<IdentityRole?> GetRoleAsync(Role role);
    Task<int> CountUsersAsync(ISpecification<AppUser> specification);
    Task<AppUser?> GetByIdAsync(string id);
    Task<bool> UpdateUserToRoleAdminAsync(AppUser user);
    string HashPassword(AppUser user, string password);
    Task<bool> UpdateUserAsync(AppUser user);
    Task<bool> DeleteUserAsync(AppUser user);
}