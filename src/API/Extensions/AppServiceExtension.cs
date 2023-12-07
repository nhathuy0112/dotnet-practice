using Application.Common.Interfaces;
using Application.Common.Services;
using Infrastructure.Repositories;

namespace API.Extensions;

public static class AppServiceExtension
{
    public static IServiceCollection AddDependency(this IServiceCollection service)
    {
        service.AddScoped<IUnitOfWork, UnitOfWork>();
        service.AddScoped<IUserRepository, UserRepository>();
        service.AddScoped<ITokenService, TokenService>();
        return service;
    }
}