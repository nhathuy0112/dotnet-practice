using System.Text.Json;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure.Data;

public class AppDataSeeding
{
    private const string BaseDevPath = "../Infrastructure/Data/SeedData";
    private const string BaseTestPath = "../../../../../src/Infrastructure/Data/SeedData";
    private const string BaseProductionPath = "SeedData";
    public static async Task SeedAsync(
        AppDbContext context, 
        IHostEnvironment environment, 
        UserManager<AppUser> userManager, 
        RoleManager<IdentityRole> roleManager)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await SeedRoleAsync(roleManager);
            await SeedAdminAsync(userManager);
            await SeedUsersAsync(userManager, amount: 5);
            await SeedCategoriesAsync(context, environment);
            await SeedProductsAsync(context, environment);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Log.Error("{@Exception}", e);
            throw new Exception("Cannot add default data");
        }
    }

    private static async Task SeedCategoriesAsync(AppDbContext context, IHostEnvironment environment)
    {
        string basePath = environment.IsProduction() ? BaseProductionPath : BaseDevPath;

        if (environment.EnvironmentName == "Testing")
        {
            basePath = BaseTestPath;
        }
        
        string path = Path.Combine(basePath, "categories.json");
        
        if (!context.Categories.Any())
        {
            var categoriesData = await File.ReadAllTextAsync(path);
            var categories = JsonSerializer.Deserialize<List<Category>>(categoriesData);
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }
    }
    
    private static async Task SeedProductsAsync(AppDbContext context, IHostEnvironment environment)
    {
        string basePath = environment.IsProduction() ? BaseProductionPath : BaseDevPath;
        
        if (environment.EnvironmentName == "Testing")
        {
            basePath = BaseTestPath;
        }
        
        string path = Path.Combine(basePath, "products.json");
        
        if (!context.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync(path);
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);
            products = products.Select(p =>
            {
                p.CreatedDate = DateTime.Now;
                p.CreatedBy = "admin@store.com";
                return p;
            }).ToList();
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
    
    private static async Task SeedAdminAsync(UserManager<AppUser> userManager)
    {
        string adminEmail = "admin@store.com";
        string adminPassword = "Abc12345!";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        
        if (admin == null)
        {
            var newAdmin = new AppUser()
            {
                UserName = adminEmail,
                Email = adminEmail
            };
            var createResult = await userManager.CreateAsync(newAdmin, adminPassword);
            var addRoleResult = await userManager.AddToRoleAsync(newAdmin, Role.Admin.ToString());

            if (createResult.Succeeded && addRoleResult.Succeeded)
            {
                return;
            }

            Log.Error("Create result: {@Result}", createResult);
            Log.Error("Add role result: {@Result}", addRoleResult);
            throw new UserException("Cannot create admin");
        }
    }

    private static async Task SeedUsersAsync(UserManager<AppUser> userManager, int amount)
    {
        string password = "Abc12345!";
        var users = new List<AppUser>();
        
        if (amount <= 0)
        {
            amount = 1;
        }

        for (int i = 1; i <= amount; i++)
        {
            users.Add(new AppUser()
            {
                Email = $"default{i}@abc.com",
                UserName = $"Default{i}",
            });
        }
        
        if (!userManager.Users.Any(u => u.UserName.Contains("Default")))
        {
            foreach (var user in users)
            {
                var createResult = await userManager.CreateAsync(user, password);
                var addRoleResult = await userManager.AddToRoleAsync(user, Role.User.ToString());

                if (!createResult.Succeeded || !addRoleResult.Succeeded)
                {
                    Log.Error("Create result: {@Result}", createResult);
                    Log.Error("Add role result: {@Result}", addRoleResult);
                    throw new UserException("Cannot create default user");
                }
            }
        }
    }
    
    private static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!roleManager.Roles.Any())
        {
            var userRole = new IdentityRole(Role.User.ToString());
            var adminRole = new IdentityRole(Role.Admin.ToString());
            var addUserRoleResult = await roleManager.CreateAsync(userRole);
            var addAdminRoleResult = await roleManager.CreateAsync(adminRole);

            if (addUserRoleResult.Succeeded && addAdminRoleResult.Succeeded)
            {
                return;
            }
            
            Log.Error("Create user role result: {@Result}", addUserRoleResult);
            Log.Error("Create admin role result: {@Result}", addAdminRoleResult);
            throw new UserException("Cannot create app roles");
        }
    }
}