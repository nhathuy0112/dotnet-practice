using MediatR;

namespace Application.Users.Commands.UpdateUserToAdmin;

public class UpdateUserToAdminCommand : IRequest<bool>
{
    public UpdateUserToAdminCommand(string userId)
    {
        Id = userId;
    }

    public string Id { get; set; }
}