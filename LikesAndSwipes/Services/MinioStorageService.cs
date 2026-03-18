using LikesAndSwipes.Models.Minio;
using LikesAndSwipes.Options;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace LikesAndSwipes.Services;

public class MinioStorageService : IMinioStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;
    private readonly ILogger<MinioStorageService> _logger;

    public MinioStorageService(IMinioClient minioClient, IOptions<MinioOptions> options, ILogger<MinioStorageService> logger)
    {
        _minioClient = minioClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<MinioObjectUploadResult> UploadAsync(IFormFile file, string? objectName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length == 0)
        {
            throw new InvalidOperationException("Cannot upload an empty file.");
        }

        var resolvedObjectName = string.IsNullOrWhiteSpace(objectName)
            ? $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}"
            : objectName.Trim();

        await EnsureBucketExistsAsync(cancellationToken);

        await using var stream = file.OpenReadStream();
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(resolvedObjectName)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
        _logger.LogInformation("Uploaded object {ObjectName} to bucket {BucketName}", resolvedObjectName, _options.BucketName);

        return new MinioObjectUploadResult
        {
            ObjectName = resolvedObjectName,
            BucketName = _options.BucketName,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            Size = file.Length
        };
    }

    public async Task<Stream?> GetAsync(string objectName, CancellationToken cancellationToken = default)
    {
        if (!await ObjectExistsAsync(objectName, cancellationToken))
        {
            return null;
        }

        var stream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName)
            .WithCallbackStream(source => source.CopyTo(stream));

        await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
        stream.Position = 0;
        return stream;
    }

    public async Task<bool> DeleteAsync(string objectName, CancellationToken cancellationToken = default)
    {
        if (!await ObjectExistsAsync(objectName, cancellationToken))
        {
            return false;
        }

        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
        _logger.LogInformation("Deleted object {ObjectName} from bucket {BucketName}", objectName, _options.BucketName);
        return true;
    }

    public async Task<string> GetPresignedUrlAsync(string objectName, CancellationToken cancellationToken = default)
    {
        if (!await ObjectExistsAsync(objectName, cancellationToken))
        {
            throw new FileNotFoundException($"Object '{objectName}' was not found in bucket '{_options.BucketName}'.");
        }

        var presignedGetObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(_options.BucketName)
            .WithObject(objectName)
            .WithExpiry(_options.PresignedUrlExpirySeconds);

        return await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
    }

    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_options.BucketName);
        var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if (exists)
        {
            return;
        }

        var makeBucketArgs = new MakeBucketArgs().WithBucket(_options.BucketName);
        await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        _logger.LogInformation("Created MinIO bucket {BucketName}", _options.BucketName);
    }

    private async Task<bool> ObjectExistsAsync(string objectName, CancellationToken cancellationToken)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectName);

            await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            return true;
        }
        catch (Minio.Exceptions.ObjectNotFoundException)
        {
            return false;
        }
    }
}
