namespace SharedKernel.Behaviors;

/// <summary>
/// Define un componente autónomo responsable de asegurar que el estado de un objeto 
/// cumpla con las precondiciones invariables de negocio antes de ser procesado.
/// </summary>
/// <typeparam name="T">La entidad, comando o estructura que se somete a inspección.</typeparam>
/// <remarks>
/// Permite externalizar la carga de validación fuera de la lógica central del caso de uso. 
/// Su diseño habilita la composición de múltiples validadores que serán evaluados 
/// en bloque por la tubería de comportamientos (Pipeline Behaviors).
/// </remarks>
public interface IValidator<in T>
{
    IEnumerable<string> Validate(T instance);
}

public record ValidationRuleResult(bool IsValid, string Message);