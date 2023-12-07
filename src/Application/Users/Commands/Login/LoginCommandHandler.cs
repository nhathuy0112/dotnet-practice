using Application.Common.Interfaces;
using Application.Common.Services;
using Domain.Exceptions;
using MediatR;

namespace Application.Users.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var userLogin = await _userRepository.LoginAsync(command.Email, command.Password);
        
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
            Email = command.Email,
            Roles = roles,
            RefreshExpiryDate = token.RefreshExpiryDate.Ticks
        };
    }
}