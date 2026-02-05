using Application.Abstracts.Services;
using Application.Options;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.Services;

public sealed class S3MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minio;
    private readonly MinioOptions _opt;

    public S3MinioFileStorageService(IMinioClient minio, IOptions<MinioOptions> options)
    {
        _minio = minio;
        _opt = options.Value;
    }
    public async Task DeleteFileAsync(string objectKey, CancellationToken ct = default)
    {
        await _minio.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_opt.Bucket)
                .WithObject(objectKey),
            ct);
    }

    public async Task<string> SaveAsync(Stream content, string fileName, string contentType, int propertyAdId, CancellationToken ct = default)
    {
        var bucket = _opt.Bucket;

        var exists = await _minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucket),
            ct);

        if (!exists)
        {
            await _minio.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucket),
                ct);
        }

        var ext = Path.GetExtension(fileName);
        var objectKey = $"{Guid.NewGuid()}{ext}";

        long size;
        if (content.CanSeek)
        {
            size = content.Length;
            content.Position = 0;
        }
        else
        {
            var ms = new MemoryStream();
            await content.CopyToAsync(ms, ct);
            ms.Position = 0;
            content = ms;
            size = ms.Length;
        }

        await _minio.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(objectKey)
                .WithStreamData(content)
                .WithObjectSize(size)
                .WithContentType(contentType),
            ct);

        return objectKey;
    }
}
