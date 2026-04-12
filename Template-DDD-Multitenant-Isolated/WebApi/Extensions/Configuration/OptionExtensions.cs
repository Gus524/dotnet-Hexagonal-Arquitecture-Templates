using SharedKernel.Configuration;

namespace WebApi.Extensions.Configuration;

public static class OptionExtensions
{
    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StaticFilesSettings>(configuration.GetSection(StaticFilesSettings.SectionName));
    }
}