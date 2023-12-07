using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Dto.Product;
using Application.Interfaces;
using Domain.Specification.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductController : BaseApiController
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [AllowAnonymous]
    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] ProductRequestParams requestParams)
    {
        return Ok(await _productService.GetProductsAsync(requestParams));
    }

    [HttpGet("product/{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        return Ok(await _productService.GetProductByIdAsync(id));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("product")]
    public async Task<IActionResult> AddProduct(ProductRequest product)
    {
        string email = User.FindFirstValue(ClaimTypes.Email);
        return Ok(await _productService.AddProductAsync(product, email));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("product/{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, ProductRequest product)
    {
        return Ok(await _productService.UpdateProductAsync(id, product));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("product/{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        return Ok(await _productService.DeleteProductAsync(id));
    }
}