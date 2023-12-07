using Application.Common.Interfaces;
using Domain.Exceptions;
using MediatR;

namespace Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
        {
            throw new UserException("User does not exist");
        }
        
        var userRoles = await _userRepository.GetRolesAsync(user);

        if (userRoles.All(r => r != "User"))
        {
            throw new UserException("Cannot delete other admin");
        }

        var result = await _userRepository.DeleteUserAsync(user);

        if (!result)
        {
            throw new UserException("Delete user fail");
        }

        return true;
    }
}