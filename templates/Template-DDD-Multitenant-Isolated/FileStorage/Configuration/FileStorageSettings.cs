using System.ComponentModel.DataAnnotations;

namespace FileStorage.Configuration;

public class FileStorageSettings
{
    public const string SectionName = "FileStorage";
    
    [Required(ErrorMessage = "La ruta base de almacenamiento es obligatoria.")]
    public string BasePath { get; set; } = string.Empty;
}