using Domain.Entities;

namespace Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Category category);
}