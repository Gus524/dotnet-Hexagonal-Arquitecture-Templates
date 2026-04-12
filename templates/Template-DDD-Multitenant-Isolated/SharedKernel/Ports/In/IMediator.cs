using SharedKernel.Mediator;
using SharedKernel.Wrappers;

namespace SharedKernel.Ports.In;

/// <summary>
/// Orquesta el despacho en memoria de intenciones (Comandos o Consultas) hacia sus respectivos 
/// manejadores, encapsulando la resolución de dependencias y el flujo a través de middlewares.
/// </summary>
/// <remarks>
/// Desacopla por completo a los invocadores (ej. Controladores REST) de los ejecutores de la lógica 
/// de aplicación, habilitando un diseño modular basado en el principio de Responsabilidad Única (SRP).
/// </remarks>
public interface IMediator
{
    Task<Response<TResponse>> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}