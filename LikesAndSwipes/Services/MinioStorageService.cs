using LikesAndSwipes.Models.Minio;
using LikesAndSwipes.Options;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System.Net;
using System.Net.Sockets;

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

        ValidateOptions();
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

        return await ExecuteWithFailoverAsync(async client =>
        {
            await EnsureBucketExistsAsync(client, cancellationToken);

            await using var stream = file.OpenReadStream();
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(GetRequiredBucketName())
                .WithObject(resolvedObjectName)
                .WithStreamData(stream)
                .WithObjectSize(file.Length)
                .WithContentType(string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType);

            await client.PutObjectAsync(putObjectArgs, cancellationToken);
            _logger.LogInformation("Uploaded object {ObjectName} to bucket {BucketName}", resolvedObjectName, GetRequiredBucketName());

            return new MinioObjectUploadResult
            {
                ObjectName = resolvedObjectName,
                BucketName = GetRequiredBucketName(),
                ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                Size = file.Length
            };
        }, cancellationToken);
    }

    public async Task<Stream?> GetAsync(string objectName, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithFailoverAsync(async client =>
        {
            if (!await ObjectExistsAsync(client, objectName, cancellationToken))
            {
                return null;
            }

            var stream = new MemoryStream();
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(GetRequiredBucketName())
                .WithObject(objectName)
                .WithCallbackStream(source => source.CopyTo(stream));

            await client.GetObjectAsync(getObjectArgs, cancellationToken);
            stream.Position = 0;
            return (Stream)stream;
        }, cancellationToken);
    }

    public async Task<bool> DeleteAsync(string objectName, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithFailoverAsync(async client =>
        {
            if (!await ObjectExistsAsync(client, objectName, cancellationToken))
            {
                return false;
            }

            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(GetRequiredBucketName())
                .WithObject(objectName);

            await client.RemoveObjectAsync(removeObjectArgs, cancellationToken);
            _logger.LogInformation("Deleted object {ObjectName} from bucket {BucketName}", objectName, GetRequiredBucketName());
            return true;
        }, cancellationToken);
    }

    public async Task<string> GetPresignedUrlAsync(string objectName, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithFailoverAsync(async client =>
        {
            if (!await ObjectExistsAsync(client, objectName, cancellationToken))
            {
                throw new FileNotFoundException($"Object '{objectName}' was not found in bucket '{GetRequiredBucketName()}'.");
            }

            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(GetRequiredBucketName())
                .WithObject(objectName)
                .WithExpiry(_options.PresignedUrlExpirySeconds);

            return await client.PresignedGetObjectAsync(presignedGetObjectArgs);
        }, cancellationToken);
    }

    private async Task EnsureBucketExistsAsync(IMinioClient client, CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(GetRequiredBucketName());
        var exists = await client.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        if (exists)
        {
            return;
        }

        var makeBucketArgs = new MakeBucketArgs().WithBucket(GetRequiredBucketName());
        await client.MakeBucketAsync(makeBucketArgs, cancellationToken);
        _logger.LogInformation("Created MinIO bucket {BucketName}", GetRequiredBucketName());
    }

    private async Task<bool> ObjectExistsAsync(IMinioClient client, string objectName, CancellationToken cancellationToken)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(GetRequiredBucketName())
                .WithObject(objectName);

            await client.StatObjectAsync(statObjectArgs, cancellationToken);
            return true;
        }
        catch (ObjectNotFoundException)
        {
            return false;
        }
    }


    private void ValidateOptions()
    {
        _ = GetRequiredBucketName();

        if (string.IsNullOrWhiteSpace(_options.Endpoint) && string.IsNullOrWhiteSpace(_options.InternalEndpoint))
        {
            throw new InvalidOperationException("At least one MinIO endpoint must be configured.");
        }

        if (string.IsNullOrWhiteSpace(_options.AccessKey))
        {
            throw new InvalidOperationException("MinIO access key is missing.");
        }

        if (string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            throw new InvalidOperationException("MinIO secret key is missing.");
        }
    }

    private string GetRequiredBucketName()
    {
        if (string.IsNullOrWhiteSpace(_options.BucketName))
        {
            throw new InvalidOperationException("MinIO bucket name is missing.");
        }

        return _options.BucketName;
    }

    private async Task<T> ExecuteWithFailoverAsync<T>(Func<IMinioClient, Task<T>> operation, CancellationToken cancellationToken)
    {
        Exception? lastException = null;

        foreach (var clientCandidate in GetClientCandidates())
        {
            try
            {
                return await operation(clientCandidate);
            }
            catch (Exception ex) when (IsEndpointConnectivityException(ex))
            {
                lastException = ex;
                _logger.LogWarning(ex, "MinIO request failed for one configured endpoint. Trying the next configured endpoint if available.");
            }
        }

        throw lastException ?? new InvalidOperationException("No MinIO endpoints are configured.");
    }

    private IEnumerable<IMinioClient> GetClientCandidates()
    {
        foreach (var endpoint in GetConfiguredEndpoints())
        {
            yield return CreateClient(endpoint);
        }

        yield return _minioClient;
    }

    private IEnumerable<string> GetConfiguredEndpoints()
    {
        var endpoints = new[] { _options.InternalEndpoint, "minio:9000", _options.Endpoint, "localhost:9000" }
            .Where(endpoint => !string.IsNullOrWhiteSpace(endpoint))
            .Where(CanResolveEndpointHost)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        foreach (var endpoint in endpoints)
        {
            yield return endpoint;
        }
    }


    private static bool CanResolveEndpointHost(string endpoint)
    {
        var normalizedEndpoint = endpoint.Contains("://", StringComparison.Ordinal)
            ? endpoint
            : $"http://{endpoint}";

        if (!Uri.TryCreate(normalizedEndpoint, UriKind.Absolute, out var uri))
        {
            return false;
        }

        try
        {
            return Dns.GetHostAddresses(uri.Host).Length > 0;
        }
        catch (SocketException)
        {
            return false;
        }
    }

    private IMinioClient CreateClient(string endpoint)
    {
        var clientBuilder = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey);

        if (_options.UseSsl)
        {
            clientBuilder = clientBuilder.WithSSL();
        }

        return clientBuilder.Build();
    }

    private static bool IsEndpointConnectivityException(Exception exception)
    {
        return exception switch
        {
            InternalClientException internalClientException => HasConnectivityMessage(internalClientException.Message),
            HttpRequestException => true,
            IOException => true,
            _ => false
        };
    }

    private static bool HasConnectivityMessage(string message)
    {
        return message.Contains("Connection refused", StringComparison.OrdinalIgnoreCase)
            || message.Contains("Name or service not known", StringComparison.OrdinalIgnoreCase)
            || message.Contains("No such host", StringComparison.OrdinalIgnoreCase)
            || message.Contains("actively refused", StringComparison.OrdinalIgnoreCase)
            || message.Contains("connection could not be established", StringComparison.OrdinalIgnoreCase);
    }
}
