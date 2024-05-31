using Serilog;

namespace API.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        Log.Information("{Time} - Request from {RequestPath}", DateTime.Now.ToString(dateTimeFormat), context.Request.Path.Value);
        await _next(context);
        Log.Information("{Time} - Response code {Code}", DateTime.Now.ToString(dateTimeFormat), context.Response.StatusCode);
    }
}