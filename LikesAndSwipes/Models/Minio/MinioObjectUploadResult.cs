namespace LikesAndSwipes.Models.Minio;

public class MinioObjectUploadResult
{
    public string ObjectName { get; init; } = string.Empty;

    public string BucketName { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    public long Size { get; init; }
}
