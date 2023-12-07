using Application.Categories.Commands.AddCategory;
using Application.Categories.Commands.UpdateCategory;
using Application.Categories.Queries.GetAllCategories;
using AutoMapper;
using Domain.Entities;

namespace Application.Categories.MappingProfile;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponse>();
        CreateMap<Category, AddCategoryResponse>();
        CreateMap<Category, UpdateCategoryResponse>();
    }
}