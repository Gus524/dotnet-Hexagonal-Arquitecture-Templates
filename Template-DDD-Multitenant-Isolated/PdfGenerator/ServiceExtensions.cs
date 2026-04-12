using System.Reflection;
using Core.Ports.Out.Reports;
using Microsoft.Extensions.DependencyInjection;
using PdfGenerator.Services;
using PdfGenerator.Templates;
using QuestPDF.Infrastructure;

namespace PdfGenerator;

public static class ServiceExtensions
{
    public static void AddPdfGenerator(this IServiceCollection services)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        services.AddScoped<IPdfPort, QuestPdfEngine>();

        var assembly = Assembly.GetExecutingAssembly();

        var templateRegistration = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
            .Where(x => x.Interface.IsGenericType &&
                        x.Interface.GetGenericTypeDefinition() == typeof(IPdfTemplateProvider<>));

        foreach (var template in templateRegistration)
        {
            services.AddScoped(template.Interface, template.Implementation);
        }
    }
}