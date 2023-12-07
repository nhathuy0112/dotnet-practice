using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Serilog;

namespace IntegrationTest.Setup;

public class TestWebApplicationFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services =>
        {
            
            var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }
            
            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer(connectionString: "Server=localhost,1433;Database=Test;User Id=sa;Password=Qwe12345@;");
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var environment = new HostingEnvironment
                {
                    EnvironmentName = "Testing"
                };
                await context.Database.MigrateAsync();
                await AppDataSeeding.SeedAsync(
                    context: context, 
                    environment: environment, 
                    userManager: userManager, 
                    roleManager: roleManager);
            }
            catch (Exception e)
            {
                Log.Error("{@Exception}", e);
            }
        });
    }
}