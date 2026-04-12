using Microsoft.Extensions.Logging;
using SharedKernel.Ports.In;
using SharedKernel.Wrappers;
using System.Diagnostics;
using SharedKernel.Mediator;

namespace SharedKernel.Behaviors;

/// <summary>
/// Intercepta el flujo de ejecución de los casos de uso para orquestar la trazabilidad, 
/// telemetría y el manejo global de excepciones de forma transversal.
/// </summary>
/// <typeparam name="TRequest">El contrato de entrada del caso de uso.</typeparam>
/// <typeparam name="TResponse">El contrato de salida estandarizado.</typeparam>
/// <remarks>
/// Elimina el código repetitivo de logs (boilerplate) y los bloques try-catch de la capa de aplicación. 
/// Mide automáticamente la latencia de cada solicitud y centraliza la captura de fallos inesperados 
/// sin ensuciar la lógica de negocio pura.
/// </remarks>
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<Response<TResponse>> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation("[INICIO] Ejecutando {RequestName}", requestName);

            var response = await next();

            stopwatch.Stop();

            if (response.Succeeded)
            {
                logger.LogInformation("[FIN] {RequestName} completado con exito en {ElapsedMilliseconds}ms",
                    requestName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.LogWarning(
                    "[ERROR] {RequestName} falló en {ElapsedMilliseconds}ms. Mensaje: {Message}. Errores: {Errors}",
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    response.Message,
                    string.Join(", ", response.Errors ?? []));
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "[EXCEPCIÓN] {RequestName} lanzó una excepción después de {ElapsedMilliseconds}ms",
                requestName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}