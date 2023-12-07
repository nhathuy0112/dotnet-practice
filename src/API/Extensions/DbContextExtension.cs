using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class DbContextExtension
{
    public static IServiceCollection AddSqlServer(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddDbContext<AppDbContext>(option =>
        {
            option.UseSqlServer(configuration.GetConnectionString("Store"), 
                builder => builder.MigrationsAssembly("Infrastructure"));
        });
        return service;
    }
}