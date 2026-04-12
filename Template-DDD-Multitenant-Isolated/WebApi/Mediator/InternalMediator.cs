using System.Collections.Concurrent;
using SharedKernel.Mediator;
using SharedKernel.Ports.In;
using SharedKernel.Wrappers;

namespace WebApi.Mediator;

public class InternalMediator(
    IServiceProvider serviceProvider
) : IMediator
{
    private static readonly ConcurrentDictionary<Type, object> WrapperCache = new();

    public Task<Response<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();

        var wrapper = (RequestHandlerBase<TResponse>)WrapperCache.GetOrAdd(requestType, type =>
        {
            var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(type, typeof(TResponse));
            return Activator.CreateInstance(wrapperType) 
                   ?? throw new InvalidOperationException($"No se pudo crear el wrapper para {type}");
        });

        return wrapper.Handle(request, serviceProvider, cancellationToken);
    }
}