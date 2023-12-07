using Application.Categories.Commands.AddCategory;
using Application.Categories.Commands.DeleteCategory;
using Application.Categories.Commands.UpdateCategory;
using Application.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class CategoryController : BaseApiController
{
    public CategoryController(IMediator mediator) : base(mediator)
    {
    }

    [AllowAnonymous]
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await _mediator.Send(new GetAllCategoriesQuery()));
    }

    [HttpPost("category")]
    public async Task<IActionResult> AddCategory(AddCategoryCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPut("category/{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        
        return Ok(await _mediator.Send(command));
    }

    [HttpDelete("category/{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        return Ok(await _mediator.Send(new DeleteCategoryCommand(id)));
    }
}