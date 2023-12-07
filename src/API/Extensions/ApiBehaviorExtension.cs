using Application.Errors;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

public static class ApiBehaviorExtension
{
    public static IServiceCollection AddApiBehaviorConfig(this IServiceCollection service)
    {
        service.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                var errors = new Dictionary<string, IEnumerable<string>>();
                var errorModels = from modelState in actionContext.ModelState
                    where modelState.Value.Errors.Any()
                    let key = modelState.Key
                    let messages = modelState.Value.Errors
                    select new {key, Messages = messages.Select(e => e.ErrorMessage)};
                
                foreach (var error in errorModels)
                {
                    errors.Add(error.key, error.Messages);
                }
                
                var errorResponses = new ValidationErrorResponse()
                {
                    Errors = errors
                };
                return new BadRequestObjectResult(errorResponses);
            };
        });
        return service;
    }
}