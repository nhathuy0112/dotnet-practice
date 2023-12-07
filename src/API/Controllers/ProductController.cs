using System.Security.Claims;
using Application.Products.Commands.AddProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProducts;
using Domain.QueryParams.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProductController : BaseApiController
{
    public ProductController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParams queryParams)
    {
        return Ok(await _mediator.Send(new GetProductsQuery(queryParams)));
    }

    [HttpGet("product/{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        return Ok(await _mediator.Send(new GetProductByIdQuery(id)));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("product")]
    public async Task<IActionResult> AddProduct(AddProductCommand command)
    {
        string email = User.FindFirstValue(ClaimTypes.Email);

        if (email != command.CreatedBy)
        {
            return BadRequest();
        }
        
        return Ok(await _mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("product/{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        
        return Ok(await _mediator.Send(command));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("product/{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        return Ok(await _mediator.Send(new DeleteProductCommand(id)));
    }
}