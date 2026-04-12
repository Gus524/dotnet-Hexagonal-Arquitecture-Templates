using Application.Dtos;
using Microsoft.Extensions.Options;
using PdfGenerator.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SharedKernel.Configuration;

namespace PdfGenerator.Templates;

public class BeneficioPdf(
    IOptions<PdfGeneratorSettings> settings,
    IOptions<StaticFilesSettings> staticFiles
) : BasePdfTemplate<BeneficioDto>(settings, staticFiles)
{
    protected override void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text($"Benefício: {Data.Parrafo}");
        });
    }
}
