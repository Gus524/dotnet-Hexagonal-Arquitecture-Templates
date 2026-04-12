using Common.Mappers;
using Prestamos.Domain.Entities;
using ProjectExample.Context;
using ProjectExample.Repository;
using ProjectExample.Tables;
using SharedKernel.Repository;

namespace ProjectExample.Prestamos.Repository;

internal class PrestatarioRepository(ProjectExampleDbContext context, IMapper<Prestatario, Fvempleado> mapper)
    : ProjectExampleRepositoryBase<Prestatario, PrestatarioId, Fvempleado>(context, mapper), IRepository<Prestatario, PrestatarioId>
{
    protected override object[] ExtractPrimaryKeyValues(PrestatarioId id)
    {
        return [Int32.Parse(id.NoEmpleado)];
    }
}