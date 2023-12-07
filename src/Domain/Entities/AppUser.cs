using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class AppUser : IdentityUser
{
    public ICollection<Token> Tokens { get; set; }
    public ICollection<IdentityUserRole<string>> UserRoles { get; set; }
}