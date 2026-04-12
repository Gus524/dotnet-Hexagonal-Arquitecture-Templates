using SharedKernel.Wrappers;

namespace SharedKernel.Mediator;

public interface IRequest<TResponse>;

public delegate Task<Response<TResponse>> RequestHandlerDelegate<TResponse>();