using System.Runtime.InteropServices;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Serilog;

namespace IntegrationTest.Setup;

public class TestWebApplicationFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    private IConfiguration _configuration;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var seperator = Path.DirectorySeparatorChar;
            var path = Directory.GetCurrentDirectory().Replace($"bin{seperator}Debug{seperator}net6.0", "Setup");
            config.AddJsonFile($"{path}{seperator}appsettings.Testing.json");
            _configuration = config.Build();
        });
        builder.ConfigureServices(async services =>
        {
            var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }
            
            services.AddDbContext<AppDbContext>(option =>
            {
                var isRunOnWindow = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                option.UseSqlServer(_configuration.GetConnectionString(isRunOnWindow ? "Window" : "Mac"));
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