using Domain.Common;

namespace Domain.Entities;

public class Token : BaseEntity
{
    public string UserId { get; set; }
    public AppUser User { get; set; }
    public string JwtToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshCreatedDate { get; set; }
    public DateTime RefreshExpiryDate { get; set; }
}