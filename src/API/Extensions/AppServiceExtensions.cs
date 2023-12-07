using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;

namespace API.Extensions;

public static class AppServiceExtensions
{
    public static IServiceCollection AddDependency(this IServiceCollection service)
    {
        service.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
        service.AddScoped<IUnitOfWork, UnitOfWork>();
        service.AddScoped<IUserService, UserService>();
        service.AddScoped<IUserRepository, UserRepository>();
        service.AddScoped<ITokenService, TokenService>();
        service.AddScoped<IProductService, ProductService>();
        service.AddScoped<ICategoryService, CategoryService>();
        return service;
    }
}