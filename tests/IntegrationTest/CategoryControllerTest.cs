using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Dto.Category;
using IntegrationTest.Setup;
using Microsoft.AspNetCore.Http;

namespace IntegrationTest;

public class CategoryControllerTest : IClassFixture<TestWebApplicationFactory<Program>>, IClassFixture<TokenFixture>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly TokenFixture _tokenFixture;
    private const string BaseUri = "/api/Category";

    public CategoryControllerTest(TestWebApplicationFactory<Program> factory, TokenFixture tokenFixture)
    {
        _factory = factory;
        _tokenFixture = tokenFixture;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Test_GetCategories()
    {
        var response = await _client.GetAsync($"{BaseUri}/categories");

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>() ?? new List<CategoryResponse>();

        Assert.Equal((HttpStatusCode) StatusCodes.Status200OK, response.StatusCode);
        Assert.NotEmpty(categories);
    }
    
    [Fact]
    public async Task Test_AddCategory_Success()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        string name = "Headphone";
        var category = new CategoryRequest()
        {
            Name = name
        };
        
        var response = await _client.PostAsJsonAsync($"{BaseUri}/category", category);
        var newCategory = await response.Content.ReadFromJsonAsync<CategoryResponse>() ?? new CategoryResponse();
        
        Assert.NotEqual(0, newCategory.Id);
        Assert.Equal(name, newCategory.Name);
    }

    [Fact]
    public async Task Test_AddCategory_Invalid()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        var category = new CategoryRequest()
        {
            Name = ""
        };
        var response = await _client.PostAsJsonAsync($"{BaseUri}/category", category);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Test_UpdateCategory_Success()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        var getAllResponse = await _client.GetAsync($"{BaseUri}/categories");

        var categories = await getAllResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>() ?? new List<CategoryResponse>();

        var categoryToUpdate = categories[0];

        var updateRequest = new CategoryRequest()
        {
            Name = "Laptop"
        };

        var response = await _client.PutAsJsonAsync($"{BaseUri}/category/{categoryToUpdate.Id}", updateRequest);
        var updatedCategory = await response.Content.ReadFromJsonAsync<CategoryResponse>() ?? new CategoryResponse();

        Assert.Equal(categoryToUpdate.Id, updatedCategory.Id);
        Assert.Equal("Laptop", updatedCategory.Name);
    }
    
    [Fact]
    public async Task Test_UpdateCategory_Invalid()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        string name = "";
        var category = new CategoryRequest()
        {
            Name = name
        };
        int id = 1;

        var response = await _client.PutAsJsonAsync($"{BaseUri}/category/{id}", category);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_UpdateCategory_NotFound()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        string name = "Keyboard";
        var category = new CategoryRequest()
        {
            Name = name
        };
        int id = 0;

        var response = await _client.PutAsJsonAsync($"{BaseUri}/category/{id}", category);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_DeleteCategory_Success()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        var getAllResponse = await _client.GetAsync($"{BaseUri}/categories?PageSize=50");

        var categories = await getAllResponse.Content.ReadFromJsonAsync<List<CategoryResponse>>() ?? new List<CategoryResponse>();

        var categoryToDelete = categories[^1];

        var response = await _client.DeleteAsync($"{BaseUri}/category/{categoryToDelete.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_DeleteCategory_NotFound()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        int id = 0;

        var response = await _client.DeleteAsync($"{BaseUri}/category/{id}");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}