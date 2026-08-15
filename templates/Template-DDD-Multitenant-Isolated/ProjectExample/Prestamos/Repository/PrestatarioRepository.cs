using System.Linq.Expressions;
using Common.Mappers;
using Common.Services.Prestamos;
using Microsoft.Extensions.DependencyInjection;
using Prestamos.Domain.Model;
using Prestamos.Domain.Ports;
using ProjectExample.Context;
using ProjectExample.Tables;
using SharedKernel.Events;
using SharedKernel.Ports.Out.MultiTenancy;

namespace ProjectExample.Prestamos.Repository;

internal class PrestatarioRepository(
    ProjectExampleDbContext context, 
    [FromKeyedServices(TenantConstants.ProjectExample)]
    IMapper<Prestatario, Fvempleado> mapper,
    IDomainEventCollector eventCollector
) : PrestatarioRepositoryBase<Fvempleado, ProjectExampleDbContext>(context, mapper, eventCollector), IPrestatarioRepository
{
    protected override object[] ExtractPrimaryKeyValues(PrestatarioId id)
    {
        return [Int32.Parse(id.NoEmpleado)];
    }

    protected override Expression<Func<Fvempleado, bool>> FilterById(PrestatarioId id) =>
        x => x.EmpNoEmpleado == int.Parse(id.NoEmpleado);
}