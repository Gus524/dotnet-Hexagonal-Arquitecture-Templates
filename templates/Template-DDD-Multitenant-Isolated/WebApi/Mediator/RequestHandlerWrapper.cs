using SharedKernel.Mediator;
using SharedKernel.Ports.In;
using SharedKernel.Wrappers;

namespace WebApi.Mediator;

internal class RequestHandlerWrapper<TRequest, TResponse> : RequestHandlerBase<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override async Task<Response<TResponse>> Handle(
        IRequest<TResponse> request, 
        IServiceProvider serviceProvider, 
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        var handlerType = typeof(IRequestHandler<TRequest, TResponse>);
        
        IRequestHandler<TRequest, TResponse>? handler = null;
        
        try
        {
            handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"No se encontró un handler registrado para: {requestType.FullName}. " +
                $"Interface esperada: {handlerType.FullName}. " +
                $"Posibles causas: (1) AddApplicationLayer() no incluyó el assembly correcto, " +
                $"(2) La clase no implementa IRequestHandler<,>, " +
                $"(3) El namespace está excluido del scan. " +
                $"Verifique ApplicationExtensions.AddApplicationLayer(assemblies).",
                ex
            );
        }

        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .ToList();

        RequestHandlerDelegate<TResponse> next = () => handler.Handle((TRequest)request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var currentNext = next;
            next = () => behavior.Handle((TRequest)request, currentNext, cancellationToken);
        }
        
        return await next();
    }
}
