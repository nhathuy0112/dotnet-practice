namespace Application.Users.Commands.Login;

public class LoginResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; }
    public long RefreshExpiryDate { get; set; }
}