using Application.Dto.Auth;
using Application.Dto.User;
using Application.Interfaces;
using Domain.Specification.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UserController : BaseApiController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest user)
    {
        return Ok(await _userService.RegisterAsync(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest user)
    {
        return Ok(await _userService.LoginAsync(user));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest tokens)
    {
        return Ok(await _userService.RefreshAsync(tokens));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        return Ok(await _userService.LogoutAsync(refreshToken));
    }
}