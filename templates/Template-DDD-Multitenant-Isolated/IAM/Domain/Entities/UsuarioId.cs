using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace IAM.Domain.Entities;

public class UsuarioId : ValueObject
{
    public string OriginKey { get; }

    public UsuarioId(string originKey)
    {
        if (string.IsNullOrWhiteSpace(originKey))
            throw new DomainException("El identificador de origen (OriginKey) no puede ser nulo o vacío.");
        
        OriginKey = originKey;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return OriginKey;
    }
}