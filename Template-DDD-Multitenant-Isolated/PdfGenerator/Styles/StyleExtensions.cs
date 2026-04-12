using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Styles;

public static class StyleExtensions
{
    public static void RowInfo(this IContainer container, string label, string text)
    {
        container.Column(column =>
        {
            column.Item().Text(label).Style(Typography.Label);
            column.Item().Text(text).Style(Typography.RowText);
        });
    }
}