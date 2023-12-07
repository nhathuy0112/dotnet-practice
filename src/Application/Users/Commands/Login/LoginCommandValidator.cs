using FluentValidation;

namespace Application.Users.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(u => u.Password)
            .NotEmpty();
    }
}