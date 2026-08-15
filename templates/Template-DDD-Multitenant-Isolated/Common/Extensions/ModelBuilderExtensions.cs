using Microsoft.EntityFrameworkCore;
using SharedKernel.Events;

namespace Common.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Metodo para configurar la tabla de EventosStore en contexto especifico,
    /// asegurando que reside en el esquema del sistema correcto
    /// </summary>
    public static void EventStoreConfig(this ModelBuilder builder)
    {
        builder.Entity<StoredEvent>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.ToTable("EventStore", "Events");
            entity.Property(x => x.Type).IsRequired().HasMaxLength(500);
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.TenantId).IsRequired().HasMaxLength(50);
            entity.Property(x => x.RetryCount).IsRequired().HasDefaultValue(0);
            entity.Property(x => x.LastError).IsRequired(false);
            entity.HasIndex(e => new { e.Processed, e.RetryCount, e.OccurredOn });
        });
    }
}