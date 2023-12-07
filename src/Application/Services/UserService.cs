using Application.Dto.Auth;
using Application.Dto.User;
using Application.Helpers;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Specification.User;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepository, ITokenService tokenService, IMapper mapper)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<bool> RegisterAsync(RegisterRequest user)
    {
        var existedUser = await _userRepository.GetByEmailAsync(user.Email);
        
        if (existedUser != null)
        {
            throw new UserException("Email existed");
        }
        
        var newUser = new AppUser()
        {
            Email = user.Email,
            UserName = user.Email
        };
        var result = await _userRepository.RegisterAsync(newUser, user.Password, Role.User);
        
        if (!result)
        {
            throw new UserException("Cannot register");
        }
        
        return true;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest user)
    {
        var userLogin = await _userRepository.LoginAsync(user.Email, user.Password);
        
        if (userLogin == null)
        {
            throw new UserException("Wrong email or password");
        }
        
        var roles = await _userRepository.GetRolesAsync(userLogin);
        var token = await _tokenService.CreateTokensAsync(userLogin);
        await _tokenService.SaveTokenAsync(token);
        return new LoginResponse()
        {
            AccessToken = token.JwtToken,
            RefreshToken = token.RefreshToken,
            Email = user.Email,
            Roles = roles,
            RefreshExpiryDate = token.RefreshExpiryDate.Ticks
        };
    }

    public async Task<RefreshResponse> RefreshAsync(RefreshRequest tokens)
    {
        var validatedToken = await _tokenService.ValidateTokensAsync(
                accessToken: tokens.AccessToken, 
                refreshToken: tokens.RefreshToken);
        
        if (validatedToken == null)
        {
            throw new UserException("Tokens are not valid");
        }

        validatedToken.JwtToken = await _tokenService.CreateAccessTokenAsync(validatedToken.User);
        await _tokenService.UpdateTokenAsync(validatedToken);
        return new RefreshResponse()
        {
            AccessToken = validatedToken.JwtToken,
            RefreshToken = validatedToken.RefreshToken,
            RefreshExpiryDate = validatedToken.RefreshExpiryDate.Ticks
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var validatedToken = await _tokenService.ValidateTokenAsync(refreshToken: refreshToken);
        
        if (validatedToken == null)
        {
            throw new UserException("Token is not valid");
        }

        await _tokenService.DeleteTokenAsync(validatedToken);
        return true;
    }

    public async Task<PaginatedResponse<UserResponse>> GetUsersAsync(UserRequestParams request)
    {
        var userRole = await _userRepository.GetRoleAsync(Role.User);
        var userByRoleSpec = new UserByRoleSpecification(userRole.Id, request);
        var userByRoleForCountSpec = new UserByRoleForCountSpecification(userRole.Id, request);
        var count = await _userRepository.CountUsersAsync(userByRoleForCountSpec);
        var users = await _userRepository.GetUsersAsync(userByRoleSpec);
        var data = _mapper.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserResponse>>(users);
        return new PaginatedResponse<UserResponse>()
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            Count = count,
            Data = data
        };
    }

    public async Task<bool> UpdateUserToRoleAdminAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new UserException("User does not exist");
        }

        var userRoles = await _userRepository.GetRolesAsync(user);

        if (userRoles.All(r => r != "User"))
        {
            throw new UserException("User is already admin");
        }
        
        var result = await _userRepository.UpdateUserToRoleAdminAsync(user);

        if (!result)
        {
            throw new UserException("Update user role to admin fail");
        }

        return true;
    }

    public async Task<UserResponse> UpdateUserAsync(string userId, UserRequest userRequest)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new UserException("User does not exist");
        }
        
        var userRoles = await _userRepository.GetRolesAsync(user);

        if (userRoles.All(r => r != "User"))
        {
            throw new UserException("Cannot update other admin");
        }

        if (user.Email != userRequest.Email)
        {
            var existedUser = await _userRepository.GetByEmailAsync(userRequest.Email);
            
            if (existedUser != null)
            {
                throw new UserException("Email existed");
            }

            user.Email = userRequest.Email;
        }

        user.PasswordHash = _userRepository.HashPassword(user, userRequest.Password);
        var updatedResult = await _userRepository.UpdateUserAsync(user);

        if (!updatedResult)
        {
            throw new UserException("User update fail");
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new UserException("User does not exist");
        }
        
        var userRoles = await _userRepository.GetRolesAsync(user);

        if (userRoles.All(r => r != "User"))
        {
            throw new UserException("Cannot delete other admin");
        }

        var result = await _userRepository.DeleteUserAsync(user);

        if (!result)
        {
            throw new UserException("Delete user fail");
        }

        return true;
    }
}