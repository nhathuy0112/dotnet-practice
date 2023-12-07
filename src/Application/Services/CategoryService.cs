using Application.Dto.Category;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync()
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        return _mapper.Map<IReadOnlyList<Category>, IReadOnlyList<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse> AddCategoryAsync(CategoryRequest category)
    {
        var newCategory = new Category()
        {
            Name = category.Name
        };
        _unitOfWork.Repository<Category>().Add(newCategory);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<CategoryResponse>(newCategory);
    }

    public async Task<CategoryResponse> UpdateCategoryAsync(int id, CategoryRequest categoryRequest)
    {
        var categoryRepo = _unitOfWork.Repository<Category>();
        var existedCategory = await categoryRepo.GetByIdAsync(id);

        if (existedCategory == null)
        {
            throw new CategoryException("Cannot find category");
        }

        existedCategory.Name = categoryRequest.Name;
        categoryRepo.Update(existedCategory);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<CategoryResponse>(existedCategory);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var categoryRepo = _unitOfWork.Repository<Category>();
        var existedCategory = await categoryRepo.GetByIdAsync(id);

        if (existedCategory == null)
        {
            throw new CategoryException("Cannot find category");
        }

        categoryRepo.Delete(existedCategory);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}