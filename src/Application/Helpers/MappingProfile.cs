using Application.Dto.Category;
using Application.Dto.Product;
using Application.Dto.User;
using AutoMapper;
using Domain.Entities;

namespace Application.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductResponse>()
            .ForMember(destination => destination.CreatedDate,
                option =>
                    option.MapFrom(source => source.CreatedDate.ToString("dd/MM/yyyy")));
        CreateMap<Category, CategoryResponse>();
        CreateMap<AppUser, UserResponse>();
    }
}