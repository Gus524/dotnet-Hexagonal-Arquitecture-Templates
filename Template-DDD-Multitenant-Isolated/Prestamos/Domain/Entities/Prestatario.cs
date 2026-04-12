using SharedKernel.Abstractions;

namespace Prestamos.Domain.Entities;

public class Prestatario : AggregateRoot<PrestatarioId>
{
    public string Nombre { get; private set; }
    public Prestatario(PrestatarioId id, string nombre) : base(id)
    {
        Nombre = nombre;
    }
}
