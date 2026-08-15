using Common.Mappers;
using Prestamos.Domain.Entities;
using ProjectExample.Tables;

namespace ProjectExample.Prestamos.Mappers;

public class PrestatarioMapper : IMapper<Prestatario, Fvempleado>
{
    public Prestatario MapToDomain(Fvempleado persistence)
    {
        var prestatrioId = new PrestatarioId(persistence.EmpNoEmpleado.ToString());
        return new Prestatario(prestatrioId, persistence.EmpNombre);
    }

    public Fvempleado MapToPersistence(Prestatario domain)
    {
        return new Fvempleado
        {
            EmpNoEmpleado = int.Parse(domain.Id.NoEmpleado),
            EmpNombre = domain.Nombre
        };
    }

    public void MapToExistingPersistence(Prestatario domain, Fvempleado persistence)
    {
        persistence.EmpNoEmpleado = int.Parse(domain.Id.NoEmpleado);
    }
}