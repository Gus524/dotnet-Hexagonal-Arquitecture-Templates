using Microsoft.Extensions.DependencyInjection;
using Shared.Services;
using SharedKernel.Ports.Out;
using SharedKernel.Ports.Out.Shared;

namespace Shared;

public static class ServiceExtensions
{
    public static void AddSharedInfraestructure(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeService, DateTimeService>();
    }
}