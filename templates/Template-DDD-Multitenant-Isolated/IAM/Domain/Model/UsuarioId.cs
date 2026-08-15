using SharedKernel.Exceptions;

namespace IAM.Domain.Model;

public readonly record struct UsuarioId
{
    public string OriginKey { get; }

    public UsuarioId(string originKey)
    {
        if (string.IsNullOrWhiteSpace(originKey))
            throw new DomainException("El identificador de origen (OriginKey) no puede ser nulo o vacío.");
        
        OriginKey = originKey;
    }

    public static UsuarioId New() => new(Guid.NewGuid().ToString());
}