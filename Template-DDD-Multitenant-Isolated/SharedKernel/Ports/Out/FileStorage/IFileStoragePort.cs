using SharedKernel.DTOs;

namespace SharedKernel.Ports.Out.FileStorage;

/// <summary>
/// Define el puerto de salida para la persistencia no estructurada (archivos binarios, imágenes, documentos), 
/// aislando a la aplicación del proveedor de almacenamiento.
/// </summary>
/// <remarks>
/// Oculta el protocolo y tecnología del sistema de archivos (ej. S3, Azure Blob, Disco local).
/// Garantiza que la capa de aplicación solo trabaje con contratos semánticos y rutas virtuales, 
/// delegando la escritura física y gestión de streams a la infraestructura periférica.
/// </remarks>
public interface IFileStoragePort
{
    Task<string> SaveFileAsync(FileUploadRequest archivo, string carpeta, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
}