namespace SharedKernel.Configuration;

public class StaticFilesSettings
{
    public const string SectionName = "Storage";
    public string StaticFilesPath { get; set; } = string.Empty;
}