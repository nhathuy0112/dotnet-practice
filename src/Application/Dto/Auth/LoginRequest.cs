using FluentValidation;

namespace Application.Dto.Auth;

public class LoginRequestValidation : AbstractValidator<LoginRequest>
{
    public LoginRequestValidation()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(u => u.Password)
            .NotEmpty();
    }
}
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}