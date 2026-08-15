namespace SharedKernel.Wrappers;

/// <summary>
/// Representa formalmente la ausencia de valor (equivalente funcional a un tipo "void"), 
/// permitiendo el uso de operaciones sin retorno dentro de flujos genéricos fuertemente tipados.
/// </summary>
/// <remarks>
/// Impide la dispersión de firmas redundantes en los manejadores de CQRS (evita la necesidad de 
/// definir interfaces <c>IRequestHandler&lt;TRequest&gt;</c> exclusivas para métodos que no devuelven nada).
/// </remarks>
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
{
    public static Unit Value => default;
    public static Task<Unit> Task => System.Threading.Tasks.Task.FromResult(default(Unit));
    public override bool Equals(object? obj) => obj is Unit;
    public bool Equals(Unit other) => true;
    public int CompareTo(Unit other) => 0;
    public int CompareTo(object? obj) => 0;
    public override int GetHashCode() => 0;
    public static bool operator ==(Unit left, Unit right) => true;
    public static bool operator !=(Unit left, Unit right) => false;
    public override string ToString() => "()";

    public static bool operator <(Unit left, Unit right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Unit left, Unit right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Unit left, Unit right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Unit left, Unit right)
    {
        return left.CompareTo(right) >= 0;
    }
}