using SharedKernel.Wrappers;

namespace SharedKernel.Mediator;

/// <summary>
/// Define el contrato funcional de un Caso de Uso atómico dentro del sistema, responsable 
/// de orquestar transacciones, mutaciones de dominio o recuperación de estado.
/// </summary>
/// <typeparam name="TRequest">La intención de negocio fuertemente tipada.</typeparam>
/// <typeparam name="TResponse">El contrato de salida estandarizado.</typeparam>
/// <remarks>
/// Fomenta la segmentación vertical del sistema. Al implementar esta interfaz, cada operación del 
/// negocio vive en su propia clase, limitando drásticamente el impacto de futuros cambios y facilitando 
/// el análisis del código.
/// </remarks>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Response<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : IRequest<TResponse>;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IRequest<TResponse>;
