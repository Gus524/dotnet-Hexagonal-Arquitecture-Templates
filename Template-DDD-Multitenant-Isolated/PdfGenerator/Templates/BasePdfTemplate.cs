using Microsoft.Extensions.Options;
using PdfGenerator.Configuration;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SharedKernel.Configuration;

namespace PdfGenerator.Templates;

public abstract class BasePdfTemplate<TData>(
    IOptions<PdfGeneratorSettings> settings,
    IOptions<StaticFilesSettings> staticFiles
) : IPdfTemplateProvider<TData> where TData : class
{
    protected readonly StaticFilesSettings StaticFiles = staticFiles.Value;
    protected readonly PdfGeneratorSettings Settings = settings.Value;
    protected TData Data { get; private set; } = null!;

    public void SetData(TData data) => Data = data;

    public DocumentMetadata GetMetaData() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(20);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(
                x => x.FontSize(12)
                    .FontFamily(Settings.FontMontserrat)
                    .FontColor(Colors.Black)
                );
            
            page.Background().Element(ComposeBackground);
            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    protected virtual void ComposeBackground(IContainer container) { }
    protected virtual void ComposeHeader(IContainer container) {}
    protected abstract void ComposeContent(IContainer container);

    protected virtual void ComposeFooter(IContainer container) { }
}