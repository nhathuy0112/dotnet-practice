using System.Reflection;
using Application.Common.Behaviors;
using MediatR;

namespace API.Extensions;

public static class MediatorExtension
{
    public static IServiceCollection AddMediatorConfig(this IServiceCollection service)
    {
        var applicationAssembly = new AssemblyName("Application");
        service.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.Load(applicationAssembly));
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });
        return service;
    }
}