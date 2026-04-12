using QuestPDF.Infrastructure;

namespace PdfGenerator.Templates;

public interface IPdfTemplateProvider<in TData> : IDocument where TData : class
{
    void SetData(TData data);
}