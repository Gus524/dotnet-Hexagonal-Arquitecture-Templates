namespace SharedKernel.Abstractions;

/// <summary>
/// Representa una Entidad dentro del modelo de dominio. 
/// Una entidad es un concepto de negocio cuya identidad se mantiene a lo largo del tiempo,
/// independientemente de los cambios en sus atributos o estado.
/// </summary>
/// <typeparam name="TId">El identificador fuertemente tipado (Value Object o primitivo) que define unívocamente a la entidad.</typeparam>
public abstract class Entity<TId>
{
    public TId Id { get; protected set; }
    
    protected Entity(TId id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }
    
    public override int GetHashCode() => Id.GetHashCode();
}