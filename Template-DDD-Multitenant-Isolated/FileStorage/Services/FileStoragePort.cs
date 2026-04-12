using FileStorage.Configuration;
using Microsoft.Extensions.Options;
using SharedKernel.DTOs;
using SharedKernel.Ports.Out;
using SharedKernel.Ports.Out.FileStorage;

namespace FileStorage.Services;

internal class FileStoragePort(
    IOptions<FileStorageSettings> options
) : IFileStoragePort
{
    private readonly FileStorageSettings _settings = options.Value;

    public async Task<string> SaveFileAsync(FileUploadRequest archivo, string carpeta, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
        var fullDirectory = Path.Combine(_settings.BasePath, carpeta);
        var finalPath = Path.Combine(fullDirectory, uniqueFileName);

        if (!Directory.Exists(fullDirectory))
        {
            Directory.CreateDirectory(fullDirectory);
        }

        await using var fileStream = new FileStream(finalPath, FileMode.Create);

        if (archivo.Content.CanSeek) 
            archivo.Content.Position = 0;
            
        await archivo.Content.CopyToAsync(fileStream, cancellationToken);

        return Path.Combine(carpeta, uniqueFileName).Replace("\\", "/");
    }

    public Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_settings.BasePath, path);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }
}