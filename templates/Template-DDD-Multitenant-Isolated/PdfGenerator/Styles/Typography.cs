using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PdfGenerator.Styles;

public static class Typography
{
    public static TextStyle Body => TextStyle.Default
        .FontSize(12)
        .FontColor(Colors.Grey.Darken3);

    public static TextStyle Label => TextStyle.Default
        .FontSize(10)
        .Bold();
    
    public static TextStyle TableHeader => TextStyle.Default
        .FontSize(10)
        .Bold()
        .FontColor(Colors.White);
    public static TextStyle RowText => TextStyle.Default
        .Medium()
        .FontSize(10);

}