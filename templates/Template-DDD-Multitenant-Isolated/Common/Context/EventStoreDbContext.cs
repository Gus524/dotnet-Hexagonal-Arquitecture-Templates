
using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Events;

namespace Common.Context;

public class EventStoreDbContext(DbContextOptions<EventStoreDbContext> options): DbContext(options)
{
    public DbSet<StoredEvent> EventStore { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.EventStoreConfig();
    }
}