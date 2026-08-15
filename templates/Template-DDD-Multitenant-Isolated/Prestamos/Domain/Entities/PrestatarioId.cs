using SharedKernel.Exceptions;

namespace Prestamos.Domain.Entities;

public readonly record struct PrestatarioId
{
    public string NoEmpleado { get; }

    public PrestatarioId(string noEmpleado)
    {
        if (string.IsNullOrWhiteSpace(noEmpleado))
            throw new DomainException("El numero de empleado es obligatorio.");
        
        NoEmpleado = noEmpleado;
    }
}