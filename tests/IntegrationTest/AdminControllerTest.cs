using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Common.Helpers;
using Application.Users.Queries.GetUsers;
using IntegrationTest.Setup;
using Microsoft.AspNetCore.WebUtilities;

namespace IntegrationTest;

public class AdminControllerTest : IClassFixture<TestWebApplicationFactory<Program>>, IClassFixture<TokenFixture>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private const string BaseUri = "/api/Admin";
    private readonly TokenFixture _tokenFixture;

    public AdminControllerTest(TestWebApplicationFactory<Program> factory, TokenFixture tokenFixture)
    {
        _factory = factory;
        _tokenFixture = tokenFixture;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenFixture.AccessToken);
    }

    [Fact]
    public async Task Test_GetUsers()
    {
        var response = await _client.GetAsync($"{BaseUri}/users");

        var users = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetUsersResponse>>() ??
                    new PaginatedResponse<GetUsersResponse>();
        
        Assert.NotEqual(0, users.Count);
    }

    [Fact]
    public async Task Test_GetUser_With_Filter()
    {
        var query = new Dictionary<string, string>()
        {
            ["Email"] = "Default"
        };

        var uri = QueryHelpers.AddQueryString($"{BaseUri}/users", query);
        
        var response = await _client.GetAsync(uri);

        var users = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetUsersResponse>>() ??
                    new PaginatedResponse<GetUsersResponse>();
        
        Assert.NotEqual(0, users.Count);
    }

    [Fact]
    public async Task Test_GetUser_With_Pagination()
    {
        var query = new Dictionary<string, string>()
        {
            ["PageIndex"] = "1",
            ["PageSize"] = "2"
        };
        
        var uri = QueryHelpers.AddQueryString($"{BaseUri}/users", query);
        
        var response = await _client.GetAsync(uri);

        var users = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetUsersResponse>>() ??
                    new PaginatedResponse<GetUsersResponse>();
        
        Assert.True(users.Data.Count <= 2);
    }

    [Fact]
    public async Task Test_UpdateRole_Admin_Success()
    {
        var getUsersResponse = await _client.GetAsync($"{BaseUri}/users");

        var users = await getUsersResponse.Content.ReadFromJsonAsync<PaginatedResponse<GetUsersResponse>>() ??
                    new PaginatedResponse<GetUsersResponse>();

        var userToUpdate = users.Data[0];

        var response = await _client.PutAsJsonAsync($"{BaseUri}/role/{userToUpdate.Id}", new { });
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_UpdateRole_Admin_User_NotFound()
    {
        var response = await _client.PutAsJsonAsync($"{BaseUri}/role/0", new { });
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Test_DeleteUser_Success()
    {
        var getUsersResponse = await _client.GetAsync($"{BaseUri}/users");

        var users = await getUsersResponse.Content.ReadFromJsonAsync<PaginatedResponse<GetUsersResponse>>() ??
                    new PaginatedResponse<GetUsersResponse>();

        var userToDelete = users.Data[0];
        
        var response = await _client.DeleteAsync($"{BaseUri}/user/{userToDelete.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_DeleteUser_NotFound()
    {
        var response = await _client.DeleteAsync($"{BaseUri}/user/0");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}