using Application.Common.Interfaces;
using AutoMapper;
using Domain.Exceptions;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.Id);

        if (user == null)
        {
            throw new UserException("User does not exist");
        }
        
        var userRoles = await _userRepository.GetRolesAsync(user);

        if (userRoles.All(r => r != "User"))
        {
            throw new UserException("Cannot update other admin");
        }

        if (user.Email != command.Email)
        {
            var existedUser = await _userRepository.GetByEmailAsync(command.Email);
            
            if (existedUser != null)
            {
                throw new UserException("Email existed");
            }

            user.Email = command.Email;
        }

        user.PasswordHash = _userRepository.HashPassword(user, command.Password);
        var updatedResult = await _userRepository.UpdateUserAsync(user);

        if (!updatedResult)
        {
            throw new UserException("User update fail");
        }

        return _mapper.Map<UpdateUserResponse>(user);
    }
}