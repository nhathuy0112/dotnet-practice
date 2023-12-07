using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(u => u.Id).NotEmpty();
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(u => u.Password)
            .NotEmpty()
            .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{6,}$")
            .WithMessage("Password at least 6 characters; password requires 1 nonAlphanumeric; 1 lower; 1 Upper.");
    }
}