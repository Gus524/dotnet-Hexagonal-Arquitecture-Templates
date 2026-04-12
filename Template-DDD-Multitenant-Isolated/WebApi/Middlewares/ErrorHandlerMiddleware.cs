using System.Net;
using System.Text.Json;
using SharedKernel.Wrappers;

namespace WebApi.Middlewares;

public class ErrorHandlerMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlerMiddleware> logger,
    IHostEnvironment env
)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            
            if (error is OperationCanceledException)
            {
                logger.LogInformation("Petición cancelada por el cliente.");
                response.StatusCode = 499;
                return;
            }

            logger.LogError(error, "Error crítico no controlado.");

            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            var responseModel = new Response<string> 
            { 
                Succeeded = false, 
                Message = env.IsDevelopment() 
                    ? $"{error.Message} | {error.StackTrace}" 
                    : "Error interno del servidor."
            };

            var result = JsonSerializer.Serialize(responseModel, 
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await response.WriteAsync(result);
        }
    }
}