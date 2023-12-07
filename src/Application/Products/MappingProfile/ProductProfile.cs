using Application.Products.Commands.AddProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetProductById;
using Application.Products.Queries.GetProducts;
using AutoMapper;
using Domain.Entities;

namespace Application.Products.MappingProfile;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, GetProductsResponse>()
            .ForMember(destination => destination.CreatedDate,
                option =>
                    option.MapFrom(source => source.CreatedDate.ToString("dd/MM/yyyy")));
        
        CreateMap<Product, AddProductResponse>()
            .ForMember(destination => destination.CreatedDate,
                option =>
                    option.MapFrom(source => source.CreatedDate.ToString("dd/MM/yyyy")));
        
        CreateMap<Product, GetProductByIdResponse>()
            .ForMember(destination => destination.CreatedDate,
                option =>
                    option.MapFrom(source => source.CreatedDate.ToString("dd/MM/yyyy")));
        
        CreateMap<Product, UpdateProductResponse>()
            .ForMember(destination => destination.CreatedDate,
                option =>
                    option.MapFrom(source => source.CreatedDate.ToString("dd/MM/yyyy")));
    }
}