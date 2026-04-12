namespace PdfGenerator.Configuration;

public class PdfGeneratorSettings
{
    public string GetImagePath(string staticFiles, string fileName) =>
        Path.Combine(staticFiles, "Pdfs", "assets", "img", fileName);
    public string FontMontserrat { get; set; } = "Montserrat";
}