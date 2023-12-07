using Application.Dto.User;
using Application.Interfaces;
using Domain.Specification.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] UserRequestParams request)
    {
        return Ok(await _userService.GetUsersAsync(request));
    }

    [HttpPut("role/{userId}")]
    public async Task<IActionResult> UpdateRoleAdmin(string userId)
    {
        return Ok(await _userService.UpdateUserToRoleAdminAsync(userId));
    }

    [HttpPut("user/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, UserRequest user)
    {
        return Ok(await _userService.UpdateUserAsync(userId, user));
    }

    [HttpDelete("user/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        return Ok(await _userService.DeleteUserAsync(userId));
    }
}