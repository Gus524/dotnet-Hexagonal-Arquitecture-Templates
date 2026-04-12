using Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Ports.Out.MultiTenancy;

namespace Identity.Contexts;

public class IdentityContext(
    DbContextOptions<IdentityContext> options,
    IConnectionResolver connectionResolver
) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        
        var connectionString = connectionResolver.GetConnectionString();
        
        optionsBuilder.UseSqlServer(connectionString,
            options => options.MigrationsHistoryTable("__EFMigrationsHistory", "Identity")
        );
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("Identity");
    }
}