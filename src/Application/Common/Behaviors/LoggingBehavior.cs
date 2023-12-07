using System.Reflection;
using MediatR;
using Serilog;

namespace Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Log.Information("Handling {@type}", typeof(TRequest).Name);
        var type = request.GetType();
        var props = new List<PropertyInfo>(type.GetProperties());
        foreach (var propertyInfo in props)
        {
            var value = propertyInfo.GetValue(request, null);
            Log.Information("{@Property}: {@Value}", propertyInfo.Name, value);
        }
        return await next();
    }
}