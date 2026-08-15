using System.Text.Json;
using Common.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedKernel.Events;
using SharedKernel.Ports.Out.MultiTenancy;

namespace Common.Outbox;

/// <summary>
/// Servicio de fondo que procesa el Outbox de eventos de dominio de forma transversal 
/// a todos los tenants, desacoplando la publicación de eventos de dominio de la 
/// transacción de negocio principal.
/// 
/// <b>Patrón Outbox:</b>
/// Los eventos de dominio se guardan en la misma transacción que la entidad (EventStore).
/// Este servicio despierta cada 5 segundos, recorre TODOS los tenants, y procesa eventos 
/// pendientes en lotes de 20 para evitar sobrecarga. Deserializa, despacha, y marca como 
/// procesados. Si falla un evento, se registra y se reintenta en la siguiente ejecución.
/// 
/// <b>Multitenancy:</b>
/// Itera sobre las connection strings configuradas (cada clave = tenant ID). Establece 
/// el tenant en ITenantProvider para cada iteración, permitiendo que los servicios 
/// inyectados resuelvan el contexto correcto. Redis se ignora automáticamente.
/// 
/// <b>Garantía de Consistencia:</b>
/// Los eventos se marcan como procesados SOLO después de que el dispatcher ejecuta sin 
/// excepciones. Si el dispatcher falla, la excepción se registra pero el evento persiste 
/// para reintentos. SaveChangesAsync() se ejecuta una sola vez por tenant, consolidando 
/// todos los eventos procesados en esa iteración.
/// </summary>
public class MultitenantOutBoxProcessor(
    IServiceScopeFactory scopeFactory, 
    IConfiguration configuration,
    ILogger<MultitenantOutBoxProcessor> logger
) : BackgroundService
{
    public const int MaxRetries = 5;

    /// <summary>
    /// Loop principal: Cada 5 segundos, itera todos los tenants y procesa su outbox.
    /// 
    /// Extrae connection strings del archivo de configuración (cada sección = tenant).
    /// Redis se excluye porque no es una base de datos de tenant.
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren();

            foreach (var connection in connectionStrings)
            {
                if (connection.Key.Equals("Redis", StringComparison.OrdinalIgnoreCase)) 
                    continue;

                var tenantId = connection.Key;
                await ProcessTenantOutboxAsync(tenantId, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    /// <summary>
    /// Procesa el outbox de eventos de un tenant específico.
    /// 
    /// Flujo:
    /// 1. Crea un scope nuevo para aislar la resolución de dependencias del tenant
    /// 2. Establece el tenant en ITenantProvider (permite que repositorios resuelvan contexto correcto)
    /// 3. Obtiene hasta 20 eventos pendientes con RetryCount, MaxRetries ordenados por fecha de ocurrencia
    /// 4. Para cada evento: deserializa, despacha a handlers, marca como procesado
    /// 5. Si falla un evento individual, registra la falla (RecordFailure), emite log y continúa
    /// 6. Persiste todos los cambios (eventos procesados y fallidos) al finalizar el lote
    /// </summary>
    private async Task ProcessTenantOutboxAsync(string tenantId, CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
        tenantProvider.SetTenantId(tenantId);
        
        var context = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
        
        var pendingEvents = await context.EventStore
            .Where(e => !e.Processed && e.RetryCount < MaxRetries)
            .OrderBy(e => e.OccurredOn)
            .Take(20)
            .ToListAsync(stoppingToken);

        if (pendingEvents.Count == 0) return;

        foreach (var storedEvent in pendingEvents)
        {
            try
            {
                var type = Type.GetType(storedEvent.Type);
                if (type == null) 
                    throw new InvalidOperationException($"No se pudo resolver el tipo {storedEvent.Type}");

                var domainEvent = (IDomainEvent)JsonSerializer.Deserialize(storedEvent.Content, type)!;

                await dispatcher.DispatchAsync(domainEvent, stoppingToken);
                
                storedEvent.MarkAsProcessed();
            }
            catch (Exception ex)
            {
                storedEvent.RecordFailure(ex.ToString());

                if (storedEvent.RetryCount >= MaxRetries)
                {
                    logger.LogError(
                        ex, 
                        "Evento {EventId} en tenant {TenantId} superó el máximo de reintentos ({MaxRetries}). Evento marcado como Dead Letter/Failed. Error: {LastError}", 
                        storedEvent.Id, 
                        tenantId, 
                        MaxRetries, 
                        storedEvent.LastError
                    );
                }
                else
                {
                    logger.LogWarning(
                        ex, 
                        "Error al procesar evento {EventId} en tenant {TenantId} (Intento {RetryCount}/{MaxRetries}).", 
                        storedEvent.Id, 
                        tenantId, 
                        storedEvent.RetryCount, 
                        MaxRetries
                    );
                }
            }
        }
        
        await context.SaveChangesAsync(stoppingToken);
    }
}
