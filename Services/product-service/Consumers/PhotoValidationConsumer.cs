using MassTransit;
using CSMVCK8S.Shared.Events;
using Microsoft.Extensions.Logging;

namespace ProductService.Consumers;

public class PhotoValidationConsumer : IConsumer<PhotoUploadEvent>
{
    private readonly ILogger<PhotoValidationConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public PhotoValidationConsumer(ILogger<PhotoValidationConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<PhotoUploadEvent> context)
    {
        var photo = context.Message;
        
        _logger.LogInformation("🔍 Fotoğraf validasyonu başladı: {PhotoId}", photo.PhotoId);

        try
        {
            // 1. GEÇİCİ VALİDASYON API'Sİ
            var validationResult = await MockValidationApi(photo.FileData);
            
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("❌ Fotoğraf validasyonu başarısız: {PhotoId} - {Message}", 
                    photo.PhotoId, validationResult.Message);
                return; // Validasyon başarısız - işlemi durdur
            }

            _logger.LogInformation("✅ Fotoğraf validasyonu başarılı: {PhotoId}", photo.PhotoId);

            // 2. Validasyon başarılı, upload için bir sonraki kuyruğa gönder
            await _publishEndpoint.Publish(new PhotoReadyForUploadEvent
            {
                PhotoId = photo.PhotoId,
                MinioObjectId = photo.MinioObjectId,
                FileData = photo.FileData,
                FileName = photo.FileName,
                ContentType = photo.ContentType
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Fotoğraf validasyon hatası: {PhotoId}", photo.PhotoId);
        }
    }

    private async Task<ValidationResult> MockValidationApi(byte[] fileData)
    {
        // GEÇİCİ API - Gerçek validasyon servisi yerine
        await Task.Delay(2000); // 2 saniye validasyon simülasyonu
        
        // Rastgele %90 başarılı, %10 başarısız
        var random = new Random();
        var isValid = random.Next(0, 10) < 9; // %90 şans
        
        return new ValidationResult
        {
            IsValid = isValid,
            Message = isValid ? "Fotoğraf geçerli" : "Fotoğraf geçersiz - test hatası"
        };
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}