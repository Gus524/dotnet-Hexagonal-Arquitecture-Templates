using SharedKernel.DTOs;

namespace WebApi.Common.Mapping;

public static class FromFileExtensions
{
    public static FileUploadRequest ToFileRequest(this IFormFile file)
    {
        return new FileUploadRequest(
            Content: file.OpenReadStream(),
            FileName: file.FileName,
            ContentType: file.ContentType,
            Length: file.Length
        );
    }
    
    public static FileUploadRequest? ToFileRequestOrNull(this IFormFile? file)
    {
        return file?.ToFileRequest();
    }
}