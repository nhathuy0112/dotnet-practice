using Application.Dto;
using Application.Dto.Auth;
using Application.Dto.Category;
using Application.Dto.Product;
using Application.Dto.User;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace API.Extensions;

public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection service)
    {
        service.AddFluentValidationAutoValidation();
        service.AddValidatorsFromAssemblyContaining<RegisterRequestValidation>();
        service.AddValidatorsFromAssemblyContaining<LoginRequestValidation>();
        service.AddValidatorsFromAssemblyContaining<RefreshRequestValidation>();
        service.AddValidatorsFromAssemblyContaining<ProductRequestValidation>();
        service.AddValidatorsFromAssemblyContaining<CategoryRequestValidation>();
        service.AddValidatorsFromAssemblyContaining<UserRequestValidation>();
        return service;
    }
}