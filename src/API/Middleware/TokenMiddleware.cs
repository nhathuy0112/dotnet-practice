using Application.Common.Interfaces;
using Domain.Exceptions;

namespace API.Middleware;

public class TokenMiddleware
{
    private readonly RequestDelegate _next;

    public TokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    private string RetrieveToken(string[] authorHeaders)
    {
        foreach (string authorHeader in authorHeaders)
        {
            if (!authorHeader.StartsWith("Bearer"))
            {
                continue;
            }
            
            return authorHeader.Replace("Bearer ", "");
        }

        throw new UserException("Token is not valid");
    }

    public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
    {
        string[] authorHeaders = context.Request.Headers.Authorization.ToArray();
        
        if (authorHeaders.Length != 0)
        {
            string token = RetrieveToken(authorHeaders);
            var existedToken = await unitOfWork.TokenRepository.GetTokenByJwtTokenAsync(token);
            
            if (existedToken == null)
            {
                throw new UserException("Token is not valid");
            }
        }

        await _next(context);
    }
}