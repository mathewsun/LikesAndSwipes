using LikesAndSwipes.Models.Minio;

namespace LikesAndSwipes.Services;

public interface IMinioStorageService
{
    Task<MinioObjectUploadResult> UploadAsync(IFormFile file, string? objectName, CancellationToken cancellationToken = default);

    Task<MinioObjectDownloadResult?> GetAsync(string objectName, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string objectName, CancellationToken cancellationToken = default);

    Task<string> GetPresignedUrlAsync(string objectName, CancellationToken cancellationToken = default);
}
