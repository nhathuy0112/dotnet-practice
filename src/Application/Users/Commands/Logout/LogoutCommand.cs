using MediatR;

namespace Application.Users.Commands.Logout;

public class LogoutCommand : IRequest<bool>
{
    public LogoutCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public string RefreshToken { get; }
}