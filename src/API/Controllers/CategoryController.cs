using Application.Dto.Category;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CategoryController : BaseApiController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await _categoryService.GetCategoriesAsync());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("category")]
    public async Task<IActionResult> AddCategory(CategoryRequest category)
    {
        return Ok(await _categoryService.AddCategoryAsync(category));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("category/{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryRequest category)
    {
        return Ok(await _categoryService.UpdateCategoryAsync(id, category));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("category/{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        return Ok(await _categoryService.DeleteCategoryAsync(id));
    }
}