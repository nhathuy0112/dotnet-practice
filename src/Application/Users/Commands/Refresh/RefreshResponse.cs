namespace Application.Users.Commands.Refresh;

public class RefreshResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public long RefreshExpiryDate { get; set; }
}