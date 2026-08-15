using Microsoft.Extensions.Logging;
using SharedKernel.Mediator;
using SharedKernel.Ports.In;
using SharedKernel.Wrappers;

namespace SharedKernel.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<Response<TResponse>> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var errors = validators
                .SelectMany(v => v.Validate(request))
                .ToList();

            if (errors.Count != 0)
            {
                logger.LogWarning(
                    "Validación fallida para request {RequestType}. Errores: {@Errors}",
                    typeof(TRequest).Name,
                    errors
                );
                return Response.Fail<TResponse>("Errores de validación", errors);
            }
        }

        logger.LogDebug("Validación exitosa para {RequestType}", typeof(TRequest).Name);
        return await next();
    }
}
