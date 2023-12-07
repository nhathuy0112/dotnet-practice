using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace API.Extensions;

public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection service)
    {
        var applicationAssembly = new AssemblyName("Application");
        service.AddFluentValidationAutoValidation();
        service.AddValidatorsFromAssembly(Assembly.Load(applicationAssembly));
        return service;
    }
}