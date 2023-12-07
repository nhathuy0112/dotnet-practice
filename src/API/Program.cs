using API.Extensions;
using API.Middleware;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.AddSeriLog(builder.Configuration);
builder.Services.AddSqlServer(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddAutoMapperConfig();
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddFluentValidation();
builder.Services.AddDependency();
builder.Services.AddMediatorConfig();
builder.Services.AddApiBehaviorConfig();

var app = builder.Build();

app.UseSwaggerDocumentation();
app.UseMiddleware(typeof(LoggingMiddleware));
app.UseMiddleware(typeof(ExceptionMiddleware));
app.UseMiddleware(typeof(TokenMiddleware));
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

#region Update db

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    // await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
    await AppDataSeeding.SeedAsync(
        context: context, 
        environment: app.Environment, 
        userManager: userManager, 
        roleManager: roleManager);
    // Only run when data was created
}
catch (Exception e)
{
    Log.Error("{@Exception}", e);
}
#endregion
app.Run();

public partial class Program {}