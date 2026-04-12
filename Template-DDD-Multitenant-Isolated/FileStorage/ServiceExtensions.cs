using FileStorage.Services;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Ports.Out;
using SharedKernel.Ports.Out.FileStorage;

namespace FileStorage;

public static class ServiceExtensions
{
    public static void AddFileStorageInfraestructure(this IServiceCollection services)
    {
        services.AddTransient<IFileStoragePort, FileStoragePort>();
    }
}