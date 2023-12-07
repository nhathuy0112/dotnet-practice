using FluentValidation;

namespace Application.Dto.User;

public class UserRequestValidation : AbstractValidator<UserRequest>
{
    public UserRequestValidation()
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
public class UserRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}