using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Repository;

namespace Common.Proxy;

/// <summary>
/// Proxy que enruta operaciones transaccionales (SaveChangesAsync) al contexto de base de datos 
/// del inquilino activo, manteniendo una interfaz simple de "guardar cambios" agnóstica del tenant.
/// 
/// Diferencia clave respecto a TenantRepositoryProxy: No valida explícitamente las excepciones 
/// de contexto HTTP. Se asume que ITenantProvider.GetTenantId() ya fue llamado exitosamente 
/// en contextos de aplicación (controllers, background jobs con SetTenantId preestablecido).
/// Si no hay TenantId disponible, falla con InvalidOperationException clara.
/// 
/// Patrón: Envoltura transparente que demora la resolución hasta que se llama SaveChangesAsync().
/// </summary>
public class TenantUnitOfWorkProxy(
    ITenantProvider tenantProvider,
    IServiceProvider serviceProvider) : IUnitOfWork
{
    private IUnitOfWork GetUnitOfWork()
    {
        string tenantId = tenantProvider.GetTenantId();
        
        return serviceProvider.GetKeyedService<IUnitOfWork>(tenantId)
            ?? throw new InvalidOperationException($"No se encontró IUnitOfWork registrado para el tenant '{tenantId}'. " +
                $"Verifique que RegisterTenantInfrastructure() fue llamado para este tenant.");
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantUnitOfWork = GetUnitOfWork();
        
        int totalSaved = 0;
        
        totalSaved += await tenantUnitOfWork.SaveChangesAsync(cancellationToken);

        var globalUnitOfWorks = serviceProvider.GetServices<IUnitOfWork>();
        foreach (var uow in globalUnitOfWorks)
        {
            if (uow != this && uow != tenantUnitOfWork)
            {
                totalSaved += await uow.SaveChangesAsync(cancellationToken);
            }
        }

        return totalSaved;
    }
}