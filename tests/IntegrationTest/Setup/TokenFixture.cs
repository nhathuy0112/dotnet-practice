using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Dto.Auth;

namespace IntegrationTest.Setup;

public class TokenFixture : IDisposable
{
    private TestWebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private string _refreshToken;
    private const string BaseUri = "api/User";
    public string AccessToken { get; private set; }
    
    public TokenFixture()
    {
        _factory = new();
        _client = _factory.CreateClient();
        Login();
    }
    
    private void Login()
    {
        var admin = new LoginRequest()
        {
            Email = "admin@store.com",
            Password = "Abc12345!"
        };
        var response = _client.PostAsJsonAsync($"{BaseUri}/login", admin).Result;
        var loginResponse = response.Content.ReadFromJsonAsync<LoginResponse>().Result ?? new LoginResponse();
        AccessToken = loginResponse.AccessToken;
        _refreshToken = loginResponse.RefreshToken;
    }

    private void Logout()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AccessToken);
        _client.PostAsJsonAsync($"{BaseUri}/logout", _refreshToken).Wait();
    }
    public void Dispose()
    {
        Logout();
        _factory.Dispose();
    }
}