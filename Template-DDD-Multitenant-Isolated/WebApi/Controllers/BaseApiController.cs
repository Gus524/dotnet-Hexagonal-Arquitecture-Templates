using Microsoft.AspNetCore.Mvc;
using SharedKernel.Wrappers;

namespace WebApi.Controllers;

/// <summary>
/// Proporciona un controlador base que unifica la respuesta HTTP para toda la API, 
/// traduciendo automáticamente los resultados del dominio a códigos de estado estándar.
/// </summary>
/// <remarks>
/// Actúa como un puente entre el patrón Result (<see cref="Response{T}"/>) del caso de uso 
/// y el protocolo HTTP. Previene que los controladores concretos contengan lógica repetitiva 
/// de evaluación condicional (if-else) para determinar qué <c>IActionResult</c> devolver, 
/// garantizando que todas las respuestas de la aplicación mantengan un formato idéntico.
/// </remarks>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Evalúa el estado de la respuesta del núcleo y lo proyecta hacia su equivalente semántico en HTTP.
    /// </summary>
    /// <typeparam name="T">El tipo de dato transportado por la envoltura de respuesta.</typeparam>
    /// <param name="response">El objeto de respuesta estandarizado proveniente de la capa de aplicación.</param>
    /// <returns>Un <see cref="IActionResult"/> configurado con el código de estado y el payload correspondientes.</returns>
    protected IActionResult HandleResult<T>(Response<T> response)
    {
        if (response.Succeeded)
        {
            return response.SuccessType switch
            {
                SuccessType.NoContent => NoContent(),
                SuccessType.Created => CreatedAtAction(string.Empty, response),
                _ => Ok(response)
            };
        }

        return response.ErrorType switch
        {
            ErrorType.NotFound => NotFound(response),
            ErrorType.Unauthorized => Unauthorized(response),
            ErrorType.Forbidden => StatusCode(403, response),
            ErrorType.Validation => BadRequest(response),
            _ => StatusCode(500, response)
        };
    }
}