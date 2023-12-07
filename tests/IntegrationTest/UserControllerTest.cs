using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Dto.Auth;
using IntegrationTest.Setup;

namespace IntegrationTest;

public class UserControllerTest : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private const string BaseUri = "/api/User";

    public UserControllerTest(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Test_Register_Success()
    {
        var newUser = new RegisterRequest()
        {
            Email = "testuser@abc.com",
            Password = "Abc12345!"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUri}/register", newUser);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Test_Register_Email_Existed()
    {
        var newUser = new RegisterRequest()
        {
            Email = "admin@store.com",
            Password = "Abc12345!"
        };
        
        var response = await _client.PostAsJsonAsync($"{BaseUri}/register", newUser);
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Test_Register_Invalid_Email_Or_Password()
    {
        var newUser = new RegisterRequest()
        {
            Email = "abc",
            Password = "1"
        };
        
        var response = await _client.PostAsJsonAsync($"{BaseUri}/register", newUser);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Test_Login_Success()
    {
        var user = new LoginRequest()
        {
            Email = "admin@store.com",
            Password = "Abc12345!"
        };
        
        var response = await _client.PostAsJsonAsync($"{BaseUri}/login", user);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse();
        
        Assert.NotNull(loginResponse.AccessToken);
        Assert.NotNull(loginResponse.RefreshToken);
        Assert.Equal(user.Email, loginResponse.Email);
        Assert.Equal("Admin", loginResponse.Roles[0]);
    }
    
    [Fact]
    public async Task Test_Login_Wrong_Email_Or_Password()
    {
        var user = new LoginRequest()
        {
            Email = "admin@store.commmmmm",
            Password = "Abc12345!"
        };
        
        var response = await _client.PostAsJsonAsync($"{BaseUri}/login", user);
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Test_Logout_Success()
    {
        var user = new LoginRequest()
        {
            Email = "admin@store.com",
            Password = "Abc12345!"
        };
        
        var response = await _client.PostAsJsonAsync($"{BaseUri}/login", user);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        var logoutResponse = await _client.PostAsJsonAsync($"{BaseUri}/logout", loginResponse.RefreshToken);
        
        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
    }

    [Fact]
    public async Task Test_Logout_Invalid_Token()
    {
        var user = new LoginRequest()
        {
            Email = "admin@store.com",
            Password = "Abc12345!"
        };
        
        var response = await _client.PostAsJsonAsync($"{BaseUri}/login", user);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

        var logoutResponse = await _client.PostAsJsonAsync($"{BaseUri}/logout", "");
        
        Assert.Equal(HttpStatusCode.InternalServerError, logoutResponse.StatusCode);
    }
    
}