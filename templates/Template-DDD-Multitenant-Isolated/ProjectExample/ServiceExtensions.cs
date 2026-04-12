using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectExample.Context;
using SharedKernel.Ports.Out.MultiTenancy;

namespace ProjectExample;

public static class ServiceExtensions
{
    public static void AddProjectExamplePersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ProjectExampleDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString(TenantConstants.ProjectExample)));

        services.RegisterTenantInfraestructure<ProjectExampleDbContext>(typeof(ProjectExampleDbContext).Assembly,
            TenantConstants.ProjectExample);
    }
}