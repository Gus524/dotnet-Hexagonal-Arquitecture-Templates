using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Prestamos.Domain.Entities;

public class PrestatarioId : ValueObject
{
    public string NoEmpleado { get; }

    public PrestatarioId(string noEmpleado)
    {
        if (string.IsNullOrWhiteSpace(noEmpleado))
            throw new DomainException("El numero de empleado es obligatorio.");
        
        NoEmpleado = noEmpleado;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return NoEmpleado;
    }
}