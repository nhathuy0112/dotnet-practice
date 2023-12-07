using Application.Users.Commands.Login;
using Application.Users.Commands.Logout;
using Application.Users.Commands.Refresh;
using Application.Users.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UserController : BaseApiController
{
    public UserController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        return Ok(await _mediator.Send(new LogoutCommand(refreshToken)));
    }
}