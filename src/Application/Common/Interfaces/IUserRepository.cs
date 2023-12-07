using Domain.Entities;
using Domain.QueryParams.User;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces;

public interface IUserRepository
{
    Task<bool> RegisterAsync(AppUser user, string password, Role role);
    Task<AppUser?> GetByEmailAsync(string email);
    Task<AppUser?> LoginAsync(string email, string password);
    Task<IList<string>> GetRolesAsync(AppUser user);
    Task<IReadOnlyList<AppUser>> GetUsersAsync(UserQueryParams queryParams, string roleId);
    Task<IdentityRole?> GetRoleAsync(Role role);
    Task<int> CountUsersAsync(UserQueryParams queryParams, string roleId);
    Task<AppUser?> GetByIdAsync(string id);
    Task<bool> UpdateUserToRoleAdminAsync(AppUser user);
    string HashPassword(AppUser user, string password);
    Task<bool> UpdateUserAsync(AppUser user);
    Task<bool> DeleteUserAsync(AppUser user);
}