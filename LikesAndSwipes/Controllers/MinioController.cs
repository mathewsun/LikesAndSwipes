using LikesAndSwipes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace LikesAndSwipes.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MinioController : ControllerBase
{
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();
    private static readonly TimeSpan BrowserCacheLifetime = TimeSpan.FromDays(365);
    private readonly IMinioStorageService _minioStorageService;

    public MinioController(IMinioStorageService minioStorageService)
    {
        _minioStorageService = minioStorageService;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? objectName, CancellationToken cancellationToken)
    {
        if (file is null)
        {
            return BadRequest("File is required.");
        }

        try
        {
            var result = await _minioStorageService.UploadAsync(file, objectName, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("download/{*objectName}")]
    public async Task<IActionResult> Download(string objectName, CancellationToken cancellationToken)
    {
        var stream = await _minioStorageService.GetAsync(objectName, cancellationToken);
        if (stream is null)
        {
            return NotFound();
        }

        Response.Headers.CacheControl = $"public,max-age={(int)BrowserCacheLifetime.TotalSeconds},immutable";
        Response.Headers.Expires = DateTimeOffset.UtcNow.Add(BrowserCacheLifetime).ToString("R");

        var contentType = ContentTypeProvider.TryGetContentType(objectName, out var detectedContentType)
            ? detectedContentType
            : "application/octet-stream";

        return File(stream, contentType);
    }

    [HttpGet("presigned-url/{*objectName}")]
    public async Task<IActionResult> GetPresignedUrl(string objectName, CancellationToken cancellationToken)
    {
        try
        {
            var url = await _minioStorageService.GetPresignedUrlAsync(objectName, cancellationToken);
            return Ok(new { ObjectName = objectName, Url = url });
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{*objectName}")]
    public async Task<IActionResult> Delete(string objectName, CancellationToken cancellationToken)
    {
        var deleted = await _minioStorageService.DeleteAsync(objectName, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
