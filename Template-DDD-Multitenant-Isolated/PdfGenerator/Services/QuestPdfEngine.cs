using Core.Ports.Out.Reports;
using Microsoft.Extensions.DependencyInjection;
using PdfGenerator.Templates;
using QuestPDF.Fluent;

namespace PdfGenerator.Services;

public class QuestPdfEngine(
    IServiceProvider serviceProvider
) : IPdfPort
{
    public Stream GeneratePdf<TData>(TData data) where TData : class
    {
        var templateProvider = serviceProvider.GetService<IPdfTemplateProvider<TData>>();

        if (templateProvider is null)
            throw new InvalidOperationException($"No se encontró una plantilla de QuestPDF para: {typeof(TData).Name}");

        templateProvider.SetData(data);

        var stream = new MemoryStream();

        templateProvider.GeneratePdf(stream);
        
        stream.Position = 0;

        return stream;
    }
}