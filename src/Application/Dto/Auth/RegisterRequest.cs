using FluentValidation;

namespace Application.Dto.Auth;

public class RegisterRequestValidation : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidation()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(u => u.Password)
            .NotEmpty()
            .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{6,}$")
            .WithMessage("Password at least 6 characters; password requires 1 nonAlphanumeric; 1 lower; 1 Upper.");
    }
}
public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}