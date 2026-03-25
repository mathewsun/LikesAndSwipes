namespace LikesAndSwipes.Models.Minio;

public class MinioObjectDownloadResult
{
    public required Stream Stream { get; init; }

    public string ContentType { get; init; } = "application/octet-stream";
}
