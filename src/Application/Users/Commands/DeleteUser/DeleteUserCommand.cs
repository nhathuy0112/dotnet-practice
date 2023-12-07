using MediatR;

namespace Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<bool>
{
    public DeleteUserCommand(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}