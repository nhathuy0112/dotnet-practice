using System.Data;
using FluentValidation;

namespace Application.Dto.Auth;

public class RefreshRequestValidation : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidation()
    {
        RuleFor(x => x.AccessToken).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
public class RefreshRequest
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}