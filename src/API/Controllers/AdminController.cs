using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Commands.UpdateUserToAdmin;
using Application.Users.Queries.GetUsers;
using Domain.QueryParams.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : BaseApiController
{
    public AdminController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] UserQueryParams query)
    {
        return Ok(await _mediator.Send(new GetUserQuery(query)));
    }

    [HttpPut("role/{userId}")]
    public async Task<IActionResult> UpdateRoleAdmin(string userId)
    {
        return Ok(await _mediator.Send(new UpdateUserToAdminCommand(userId)));
    }

    [HttpPut("user/{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, UpdateUserCommand command)
    {
        if (userId != command.Id)
        {
            return BadRequest();
        }
        
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("user/{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        return Ok(await _mediator.Send(new DeleteUserCommand(userId)));
    }

    
}