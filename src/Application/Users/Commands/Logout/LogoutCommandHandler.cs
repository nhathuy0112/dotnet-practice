using Application.Common.Interfaces;
using Application.Common.Services;
using Domain.Exceptions;
using MediatR;

namespace Application.Users.Commands.Logout;


public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly ITokenService _tokenService;

    public LogoutCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<bool> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var validatedToken = await _tokenService.ValidateTokenAsync(refreshToken: command.RefreshToken);
        
        if (validatedToken == null)
        {
            throw new UserException("Token is not valid");
        }

        await _tokenService.DeleteTokenAsync(validatedToken);
        return true;
    }
}