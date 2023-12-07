using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    private IUserRepository _userRepository;

    public RegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var existedUser = await _userRepository.GetByEmailAsync(command.Email);
        
        if (existedUser != null)
        {
            throw new UserException("Email existed");
        }
        
        var newUser = new AppUser()
        {
            Email = command.Email,
            UserName = command.Email
        };
        var result = await _userRepository.RegisterAsync(newUser, command.Password, Role.User);
        
        if (!result)
        {
            throw new UserException("Cannot register");
        }
        
        return true;
    }
}