using Application.Dto.Auth;
using Application.Dto.User;
using Application.Helpers;
using Domain.Entities;
using Domain.Specification.User;

namespace Application.Interfaces;

public interface IUserService
{
    Task<bool> RegisterAsync(RegisterRequest user);
    Task<LoginResponse> LoginAsync(LoginRequest user);
    Task<RefreshResponse> RefreshAsync(RefreshRequest tokens);
    Task<bool> LogoutAsync(string refreshToken);
    Task<PaginatedResponse<UserResponse>> GetUsersAsync(UserRequestParams request);
    Task<bool> UpdateUserToRoleAdminAsync(string userId);
    Task<UserResponse> UpdateUserAsync(string userId, UserRequest user);
    Task<bool> DeleteUserAsync(string userId);
}