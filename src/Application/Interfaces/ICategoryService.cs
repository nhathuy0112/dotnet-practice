using Application.Dto.Category;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync();
    Task<CategoryResponse> AddCategoryAsync(CategoryRequest category);
    Task<CategoryResponse> UpdateCategoryAsync(int id, CategoryRequest categoryRequest);
    Task<bool> DeleteCategoryAsync(int id);
}