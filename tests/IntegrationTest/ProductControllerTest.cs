using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Common.Helpers;
using Application.Products.Commands.AddProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProducts;
using IntegrationTest.Setup;
using Microsoft.AspNetCore.WebUtilities;

namespace IntegrationTest;

public class ProductControllerTest : IClassFixture<TestWebApplicationFactory<Program>>, IClassFixture<TokenFixture>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory<Program> _factory;
    private readonly TokenFixture _tokenFixture;
    private const string BaseUri = "/api/Product";

    public ProductControllerTest(TestWebApplicationFactory<Program> factory, TokenFixture tokenFixture)
    {
        _factory = factory;
        _tokenFixture = tokenFixture;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Test_GetProducts()
    {
        var response = await _client.GetAsync($"{BaseUri}/products");

        var products = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetProductsResponse>>() ??
                         new PaginatedResponse<GetProductsResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEqual(0, products.Count);
    }

    [Fact]
    public async Task Test_GetProducts_With_Filter()
    {
        var query = new Dictionary<string, string>()
        {
            ["CategoryId"] = "2",
            ["Sort"] = "price-desc"
        };
        var uri = QueryHelpers.AddQueryString($"{BaseUri}/products", query);
        
        var response = await _client.GetAsync(uri);
        
        var products = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetProductsResponse>>() ??
                       new PaginatedResponse<GetProductsResponse>();
        
        Assert.True(products.Data.All(p => p.CategoryId == 2));
        Assert.True(products.Data[0].Price >= products.Data[1].Price);
    }

    [Fact]
    public async Task Test_GetProducts_Filter_By_PriceMin_PriceMax()
    {
        var query = new Dictionary<string, string>()
        {
            ["PriceMin"] = "200",
            ["PriceMax"] = "250"
        };
        var uri = QueryHelpers.AddQueryString($"{BaseUri}/products", query);
        
        var response = await _client.GetAsync(uri);
        
        var products = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetProductsResponse>>() ??
                       new PaginatedResponse<GetProductsResponse>();
        
        Assert.DoesNotContain(products.Data, p => p.Price is < 200 or > 250);
    }

    [Fact]
    public async Task Test_GetProducts_With_Pagination()
    {
        var query = new Dictionary<string, string>()
        {
            ["PageIndex"] = "1",
            ["PageSize"] = "3"
        };
        var uri = QueryHelpers.AddQueryString($"{BaseUri}/products", query);
        
        var response = await _client.GetAsync(uri);
        
        var productsFirstPage = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetProductsResponse>>() ??
                       new PaginatedResponse<GetProductsResponse>();

        Assert.True(productsFirstPage.Data.Count <= 3);
    }

    [Fact]
    public async Task Test_GetProducts_With_Invalid_PageIndex_Max_PageSize()
    {
        var query = new Dictionary<string, string>()
        {
            ["PageIndex"] = "-1",
            ["PageSize"] = "100"
        };
        
        var uri = QueryHelpers.AddQueryString($"{BaseUri}/products", query);
        
        var response = await _client.GetAsync(uri);
        
        var products = await response.Content.ReadFromJsonAsync<PaginatedResponse<GetProductsResponse>>() ??
                                new PaginatedResponse<GetProductsResponse>();
        
        Assert.Equal(1, products.PageIndex);
        Assert.Equal(50, products.PageSize);
        Assert.NotEmpty(products.Data);
    }

    [Fact]
    public async Task Test_GetProductById_Success()
    {
        var getAllResponse = await _client.GetAsync($"{BaseUri}/products");

        var products = await getAllResponse.Content.ReadFromJsonAsync<PaginatedResponse<GetProductsResponse>>() ??
                       new PaginatedResponse<GetProductsResponse>();

        var productToGet = products.Data[0];
        var response = await _client.GetAsync($"{BaseUri}/product/{productToGet.Id}");

        var product = await response.Content.ReadFromJsonAsync<GetProductByIdResponse>() ?? new GetProductByIdResponse();
        
        Assert.Equal(productToGet.Id, product.Id);
    }
    
    [Fact]
    public async Task Test_GetProductById_NotFound()
    {
        int id = 0;
        
        var response = await _client.GetAsync($"{BaseUri}/product/{id}");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Test_AddProduct_Success()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        var newProduct = new AddProductCommand()
        {
            CategoryId = 2,
            Name = "Phone",
            Price = 200,
            CreatedBy = "admin@store.com"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUri}/product", newProduct);

        var product = await response.Content.ReadFromJsonAsync<AddProductResponse>() ?? new AddProductResponse();
        
        Assert.NotEqual(0, product.Id);
        Assert.Equal(newProduct.Name, product.Name);
        Assert.Equal(newProduct.CategoryId, product.CategoryId);
        Assert.Equal(newProduct.Price, product.Price);
    }

    [Fact]
    public async Task Test_AddProduct_CategoryId_Invalid()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        var newProduct = new AddProductCommand()
        {
            CategoryId = 100,
            Name = "Phone",
            Price = 200,
            CreatedBy = "admin@store.com"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUri}/product", newProduct);
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_AddProduct_Invalid_Name_Or_Price()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        var newProduct = new AddProductCommand()
        {
            CategoryId = 2,
            Name = "",
            Price = -1,
            CreatedBy = "admin@store.com"
        };

        var response = await _client.PostAsJsonAsync($"{BaseUri}/product", newProduct);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_UpdateProduct_Success()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        var getAllResponse = await _client.GetAsync($"{BaseUri}/products");

        var products = await getAllResponse.Content.ReadFromJsonAsync<PaginatedResponse<GetProductsResponse>>() ??
                       new PaginatedResponse<GetProductsResponse>();

        var productToUpdate = products.Data[0];

        string nameToUpdate = "Macbook";
        var updateRequest = new UpdateProductCommand()
        {
            Id = productToUpdate.Id,
            CategoryId = productToUpdate.CategoryId,
            Name = nameToUpdate,
            Price = productToUpdate.Price
        };

        var response = await _client.PutAsJsonAsync($"{BaseUri}/product/{productToUpdate.Id}", updateRequest);

        var product = await response.Content.ReadFromJsonAsync<UpdateProductResponse>() ?? new UpdateProductResponse();
        
        Assert.Equal(productToUpdate.Id, product.Id);
        Assert.Equal(nameToUpdate, product.Name);
    }
    
    [Fact]
    public async Task Test_UpdateProduct_Category_Invalid()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        int productId = 10;
        var updatedProduct = new UpdateProductCommand()
        {
            Id = productId,
            CategoryId = 100,
            Name = "Phone",
            Price = 200
        };

        var response = await _client.PutAsJsonAsync($"{BaseUri}/product/{productId}", updatedProduct);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_UpdateProduct_Product_NotFound()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        int productId = 100;
        var updatedProduct = new UpdateProductCommand()
        {
            Id = productId,
            CategoryId = 100,
            Name = "Phone",
            Price = 200
        };

        var response = await _client.PutAsJsonAsync($"{BaseUri}/product/{productId}", updatedProduct);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_UpdateProduct_Invalid_Name_Or_Price()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        int productId = 10;
        var updatedProduct = new UpdateProductCommand()
        {
            Id = productId,
            CategoryId = 100,
            Name = "",
            Price = -2
        };

        var response = await _client.PutAsJsonAsync($"{BaseUri}/product/{productId}", updatedProduct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Test_DeleteProduct_Success()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        var getAllResponse = await _client.GetAsync($"{BaseUri}/products");

        var products = await getAllResponse.Content.ReadFromJsonAsync<PaginatedResponse<GetProductsResponse>>() ??
                       new PaginatedResponse<GetProductsResponse>();

        var productToDelete = products.Data[^1];
        
        var response = await _client.DeleteAsync($"{BaseUri}/product/{productToDelete.Id}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Test_DeleteProduct_NotFound()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _tokenFixture.AccessToken);
        int productId = 0;
        
        var response = await _client.DeleteAsync($"{BaseUri}/product/{productId}");
        
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}