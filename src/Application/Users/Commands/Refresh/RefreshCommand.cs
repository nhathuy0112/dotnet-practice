using MediatR;

namespace Application.Users.Commands.Refresh;

public class RefreshCommand : IRequest<RefreshResponse>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}