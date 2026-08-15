using Common.Mappers;
using Common.Repository;
using Microsoft.EntityFrameworkCore;
using Prestamos.Domain.Entities;
using SharedKernel.Events;

namespace Common.Services.Prestamos;

public abstract class PrestatarioRepositoryBase<TPersistence, TContext>(
    TContext context,
    IMapper<Prestatario, TPersistence> mapper,
    IDomainEventCollector eventCollector
) : GenericRepository<Prestatario, PrestatarioId, TPersistence, TContext>(context, mapper, eventCollector)
    where TPersistence : class
    where TContext : DbContext;