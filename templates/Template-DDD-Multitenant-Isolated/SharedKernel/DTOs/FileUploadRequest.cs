namespace SharedKernel.DTOs;

public record FileUploadRequest(Stream Content, string FileName, string ContentType, long Length);