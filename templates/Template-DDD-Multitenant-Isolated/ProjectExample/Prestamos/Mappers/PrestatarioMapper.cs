using Common.Mappers;
using Prestamos.Domain.Entities;
using ProjectExample.Tables;

namespace ProjectExample.Prestamos.Mappers;

public class PrestatarioMapper : IMapper<Prestatario, Fvempleado>
{
    public Prestatario Map(Fvempleado persistence)
    {
        return new Prestatario(new PrestatarioId(persistence.EmpNoEmpleado.ToString()), persistence.EmpNombre);
    }

    public Fvempleado Map(Prestatario domain)
    {
        return new Fvempleado
        {
            EmpNoEmpleado = int.Parse(domain.Id.NoEmpleado),
            EmpNombre = domain.Nombre
        };
    }
}