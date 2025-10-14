using MassTransit;
using CSMVCK8S.Shared.Events;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Model;

namespace UserService.Consumers;

public class PhotoStatusConsumer : IConsumer<PhotoUploadCompletedEvent>
{
    private readonly UserDbContext _context;
    private readonly ILogger<PhotoStatusConsumer> _logger;

    public PhotoStatusConsumer(UserDbContext context, ILogger<PhotoStatusConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PhotoUploadCompletedEvent> context)
    {
        var statusEvent = context.Message;
        
        _logger.LogInformation("🔄 Fotoğraf status güncelleniyor: {PhotoId} -> {Status}", 
            statusEvent.PhotoId, statusEvent.Status);

        try
        {
            var photo = await _context.Photos.FindAsync(statusEvent.PhotoId);
            if (photo != null)
            {
                photo.Status = statusEvent.Status;
                photo.MinioUrl = statusEvent.MinioUrl;
                photo.ErrorMessage = statusEvent.ErrorMessage;
                photo.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("✅ Fotoğraf status güncellendi: {PhotoId} -> {Status}", 
                    statusEvent.PhotoId, statusEvent.Status);
            }
            else
            {
                _logger.LogWarning("❌ Fotoğraf bulunamadı: {PhotoId}", statusEvent.PhotoId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Fotoğraf status güncelleme hatası: {PhotoId}", statusEvent.PhotoId);
            throw;
        }
    }
}