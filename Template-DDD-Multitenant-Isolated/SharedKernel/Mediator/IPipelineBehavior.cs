using SharedKernel.Ports.In;
using SharedKernel.Wrappers;

namespace SharedKernel.Mediator;

public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Response<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
