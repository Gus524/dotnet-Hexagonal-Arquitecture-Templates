using Identity.Contexts;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SharedKernel.Ports.Out;
using SharedKernel.Ports.Out.MultiTenancy;
using SharedKernel.Ports.Out.Repository;

namespace Identity.Initializer;

public class IdentityInitializer(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<IdentityInitializer> logger
) : IDbContextInitializer
{
    public async Task InitializeAsync()
    {
        var connectionStrings = configuration.GetSection("ConnectionStrings").GetChildren();

        foreach (var connection in connectionStrings)
        {
            logger.LogInformation("Iniciando migración de Identity para: {Tenant}", connection.Key);

            using var scope = scopeFactory.CreateScope();
            
            var scopedTenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();
            scopedTenantProvider.SetTenantId(connection.Key);
            
            var scopedContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            var scopedUserManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var scopedRoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            try
            {
                logger.LogInformation("Ejecutando MigrateAsync en: {DB}", scopedContext.Database.GetConnectionString());
                
                await scopedContext.Database.MigrateAsync();
                
                await SeedRoles(scopedRoleManager);
                await SeedUsers(scopedUserManager);

                logger.LogInformation("Migración exitosa para: {Tenant}", connection.Key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al migrar la base de datos del tenant {Tenant}", connection.Key);
            }
        }
    }

    private async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        var roles = IdentityRoles.AllRoles;

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private async Task SeedUsers(UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser
        {
            UserName = "Admin",
            Email = "admin@admin.com",
            NombreCompleto = "Administrador",
            OriginKey = "admin"
        };
        
        await userManager.CreateAsync(user, "Password123!");
        await userManager.AddToRoleAsync(user, IdentityRoles.Administrador);
    }
}