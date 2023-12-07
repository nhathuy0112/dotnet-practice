using System.Text;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Specification.Token;

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
            var tokenByJwtSpec = new TokenByJwtSpecification(token);
            var existedToken = await unitOfWork.Repository<Token>().GetAsync(tokenByJwtSpec);
        
            if (existedToken == null)
            {
                throw new UserException("Token is not valid");
            }
        }

        await _next(context);
    }
}