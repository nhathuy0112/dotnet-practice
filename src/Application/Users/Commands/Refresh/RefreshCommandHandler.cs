using Application.Common.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Users.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshResponse>
{
    private readonly ITokenService _tokenService;

    public RefreshCommandHandler(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<RefreshResponse> Handle(RefreshCommand command, CancellationToken cancellationToken)
    {
        var validatedToken = await _tokenService.ValidateTokensAsync(
            accessToken: command.AccessToken, 
            refreshToken: command.RefreshToken);
        
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
}