using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<UpdateUserResponse>
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}