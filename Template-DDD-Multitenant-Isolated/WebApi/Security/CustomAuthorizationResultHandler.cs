using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using SharedKernel.Wrappers;

namespace WebApi.Security;

public class CustomAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _resultHandler = new();
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Succeeded && authorizeResult.Forbidden)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            
            var responseModel = Response<string>.Forbbiden("Acceso denegado. No tiene los permisos requeridos.");
            
            var result = JsonSerializer.Serialize(responseModel);
            await context.Response.WriteAsync(result);
            return;
        }
        
        if (authorizeResult.Challenged)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            var responseModel = Response<string>.Unauthorized("Usted no esta autorizado para consumir este recurso.");
            await context.Response.WriteAsync(JsonSerializer.Serialize(responseModel));
            return;
        }
        
        await _resultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}