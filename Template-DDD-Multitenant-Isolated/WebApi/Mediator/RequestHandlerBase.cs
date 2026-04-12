using SharedKernel.Mediator;
using SharedKernel.Ports.In;
using SharedKernel.Wrappers;

namespace WebApi.Mediator;

public abstract class RequestHandlerBase<TResponse>
{
    public abstract Task<Response<TResponse>> Handle(
        IRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    );
}