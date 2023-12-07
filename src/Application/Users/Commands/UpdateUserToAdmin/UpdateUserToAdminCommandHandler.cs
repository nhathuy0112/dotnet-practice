using Application.Common.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Users.Commands.UpdateUserToAdmin;

public class UpdateUserToAdminCommandHandler : IRequestHandler<UpdateUserToAdminCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserToAdminCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(UpdateUserToAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
        {
            throw new UserException("User does not exist");
        }

        var userRoles = await _userRepository.GetRolesAsync(user);

        if (userRoles.All(r => r != "User"))
        {
            throw new UserException("User is already admin");
        }
        
        var result = await _userRepository.UpdateUserToRoleAdminAsync(user);

        if (!result)
        {
            throw new UserException("Update user role to admin fail");
        }

        return true;
    }
}