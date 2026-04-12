using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Styles;

public static class DesignSystem
{
    public static IContainer ApplyRowStyle(this IContainer container, bool hasBack)
    {
        var background = hasBack ? Colors.Indigo.Lighten5 : Colors.White;

        return container.Background(background)
            .Border(1)
            .BorderColor(Colors.Indigo.Lighten2)
            .Padding(1)
            .ShowOnce();
    }

    public static void TextColumn(this RowDescriptor row, string text)
    {
        row.RelativeItem().AlignMiddle().Text(text);
    }
    
    public static void ValueColumn(this RowDescriptor row, string text, float width = 90)
    {
        row.ConstantItem(width).AlignMiddle().AlignCenter().Text(text);
    }
}