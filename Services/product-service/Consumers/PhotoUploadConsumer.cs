using MassTransit;
using CSMVCK8S.Shared.Events;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace ProductService.Consumers;

public class PhotoUploadConsumer : IConsumer<PhotoReadyForUploadEvent>
{
    private readonly ILogger<PhotoUploadConsumer> _logger;
    private readonly IMinioClient _minioClient;
    private readonly IPublishEndpoint _publishEndpoint; // ⬅️ BUNU EKLE

    public PhotoUploadConsumer(
        ILogger<PhotoUploadConsumer> logger, 
        IMinioClient minioClient,
        IPublishEndpoint publishEndpoint) // ⬅️ BUNU EKLE
    {
        _logger = logger;
        _minioClient = minioClient;
        _publishEndpoint = publishEndpoint; // ⬅️ BUNU EKLE
    }

    public async Task Consume(ConsumeContext<PhotoReadyForUploadEvent> context)
    {
        var photo = context.Message;
        
        _logger.LogInformation("📤 MinIO'ya fotoğraf yükleniyor: {PhotoId}", photo.PhotoId);

        try
        {
            // 1. Bucket kontrolü ve oluşturma
            var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket("photos"));
            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket("photos"));
                _logger.LogInformation("✅ 'photos' bucket oluşturuldu");
            }

            // 2. Fotoğrafı MinIO'ya yükle
            using var stream = new MemoryStream(photo.FileData);
            
            await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket("photos")
                .WithObject(photo.MinioObjectId)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(photo.ContentType));

            _logger.LogInformation("✅ Fotoğraf MinIO'ya yüklendi: {PhotoId} - {ObjectId}", 
                photo.PhotoId, photo.MinioObjectId);

            // 3. ✅ STATUS GÜNCELLE: Başarılı event gönder
            _logger.LogInformation("📨 PhotoUploadCompletedEvent gönderiliyor: {PhotoId}", photo.PhotoId);
            
            await _publishEndpoint.Publish(new PhotoUploadCompletedEvent // ⬅️ _publishEndpoint kullan
            {
                PhotoId = photo.PhotoId,
                Status = "completed",
                MinioUrl = $"http://minio-service:9000/photos/{photo.MinioObjectId}"
            });

            _logger.LogInformation("✅ PhotoUploadCompletedEvent gönderildi: {PhotoId}", photo.PhotoId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ MinIO'ya fotoğraf yükleme hatası: {PhotoId}", photo.PhotoId);
            
            // 4. ✅ HATA DURUMU: Failed event gönder
            await _publishEndpoint.Publish(new PhotoUploadCompletedEvent // ⬅️ _publishEndpoint kullan
            {
                PhotoId = photo.PhotoId,
                Status = "failed",
                ErrorMessage = ex.Message
            });

            _logger.LogInformation("📨 Failed event gönderildi: {PhotoId}", photo.PhotoId);
        }
    }
}