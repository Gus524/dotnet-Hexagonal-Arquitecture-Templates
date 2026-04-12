using Common.Mappers;
using Common.Repository;
using ProjectExample.Context;
using SharedKernel.Abstractions;

namespace ProjectExample.Repository;

/// <summary>
/// Especializa el repositorio genérico para enlazarlo estáticamente con el contexto de base de datos de este inquilino,
/// resolviendo de forma implícita la dependencia de infraestructura y permitiendo que los repositorios derivados 
/// mantengan firmas de constructor limpias.
/// </summary>
internal abstract class ProjectExampleRepositoryBase<TAggregate, TId, TPersistence>(
    ProjectExampleDbContext context, 
    IMapper<TAggregate, TPersistence> mapper
) : GenericRepository<TAggregate, TId, TPersistence, ProjectExampleDbContext>(context, mapper)
    where TAggregate : class, IAggregateRoot
    where TPersistence : class;