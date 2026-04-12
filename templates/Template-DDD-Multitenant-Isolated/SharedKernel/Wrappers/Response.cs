using System.Text.Json.Serialization;

namespace SharedKernel.Wrappers;

/// <summary>
/// Implementa el patrón Result para estandarizar el contrato de salida de todas las 
/// operaciones de la capa de aplicación, encapsulando tanto éxitos como fallos.
/// </summary>
/// <typeparam name="T">El tipo de dato de retorno (payload) esperado en caso de éxito.</typeparam>
/// <remarks>
/// Oculta la complejidad de la comunicación de errores entre capas. Previene el lanzamiento de 
/// excepciones para controlar el flujo de la aplicación al retornar explícitamente el estado 
/// de la operación, tipologías de error y mensajes de validación estandarizados, listos 
/// para ser interpretados o mapeados a códigos HTTP por la capa web.
/// </remarks>
public class Response<T>
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public T?  Data { get; set; }
    [JsonIgnore]
    public SuccessType SuccessType { get; set; }
    [JsonIgnore]
    public ErrorType ErrorType { get; set; }
    public Response() { }

    private Response(bool succeeded, T? data, string? message, List<string>? errors, SuccessType successType,
        ErrorType errorType)
    {
        Succeeded = succeeded;
        Message = message;
        Data = data;
        Errors = errors ?? [];
        SuccessType = successType;
        ErrorType = errorType;
    }

    public static Response<T> Success(T data, string? message = null)
    {
        return new Response<T>(true, data, message, null, SuccessType.Ok, default);
    }
    
    public static Response<Unit> NoContent(string? message = null)
    {
        return new Response<Unit>(true, Unit.Value, message, null, SuccessType.NoContent, default);
    }
    
    public static Response<T> Created(T data, string? message = null)
    {
        return new Response<T>(true, data, message, null, SuccessType.Created, default);
    }
    
    public static Response<T> Fail(string errorMessage, List<string> errors)
    {
        return new Response<T>(false, default, errorMessage, errors, default, ErrorType.Validation);
    }

    public static Response<T> Fail(string errorMessage)
    {
        return new Response<T>(false, default, errorMessage, null, default, ErrorType.Validation);
    }
    
    public static Response<T> NotFound(string errorMessage = "Recurso no encontrado")
    {
        return new Response<T>(false, default, errorMessage, [errorMessage], default, ErrorType.NotFound);
    }
    
    public static Response<T> Unauthorized(string errorMessage = "Usted no está autorizado para consumir este recurso.")
    {
        return new Response<T>(false, default, errorMessage, [errorMessage], default, ErrorType.Unauthorized);
    }

    public static Response<T> BusinessFail(string errorMesage)
    {
        return new Response<T>(false, default, errorMesage, [], default, ErrorType.BusinessLogic);
    }
    
    public static Response<T> Forbbiden(string errorMessage = "No tiene los permisos suficientes para consumir este recurso.")
    {
        return new Response<T>(false, default, errorMessage, [errorMessage], default, ErrorType.Forbidden);
    }
}