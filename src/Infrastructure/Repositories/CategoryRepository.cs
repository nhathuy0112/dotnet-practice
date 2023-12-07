using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
    {
        return await GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public Task AddCategoryAsync(Category category)
    {
        Add(category);
        return Task.CompletedTask;
    }

    public Task UpdateCategoryAsync(Category category)
    {
        Update(category);
        return Task.CompletedTask;
    }

    public Task DeleteCategoryAsync(Category category)
    {
        Delete(category);
        return Task.CompletedTask;
    }
}