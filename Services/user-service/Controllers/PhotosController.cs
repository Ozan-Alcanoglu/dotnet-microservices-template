using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSMVCK8S.Shared.Events;
using MassTransit;
using UserService.Model;
using UserService.Data;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotosController : ControllerBase
{
    private readonly UserDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PhotosController> _logger;

    public PhotosController(UserDbContext context, IPublishEndpoint publishEndpoint, ILogger<PhotosController> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> UploadPhoto(IFormFile photo)
    {
        try
        {
            // 1. Hızlı validasyon
            if (photo == null || photo.Length == 0)
                return BadRequest("Fotoğraf boş olamaz");

            if (photo.Length > 10 * 1024 * 1024) // 10MB
                return BadRequest("Fotoğraf çok büyük");

            // 2. Basit Photo kaydı (SADECE REFERANS)
            var photoRecord = new Photo
            {
                UserId = "current-user", // Gerçek uygulamada auth'dan al
                FileName = photo.FileName,
                MinioObjectId = Guid.NewGuid().ToString(),
                FileSize = photo.Length,
                Status = "processing"
            };

            _context.Photos.Add(photoRecord);
            await _context.SaveChangesAsync();

            // 3. Fotoğrafı byte array'e çevir
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await photo.CopyToAsync(memoryStream);
                fileData = memoryStream.ToArray();
            }

            // 4. RabbitMQ'ya event gönder
            var photoEvent = new PhotoUploadEvent
            {
                PhotoId = photoRecord.Id,
                MinioObjectId = photoRecord.MinioObjectId,
                FileName = photo.FileName,
                FileData = fileData,
                UserId = photoRecord.UserId,
                ContentType = photo.ContentType,
                FileSize = photo.Length
            };

            await _publishEndpoint.Publish(photoEvent);

            _logger.LogInformation("📸 Fotoğraf kuyruğa eklendi: {PhotoId}", photoRecord.Id);

            // 5. Hemen yanıt dön
            return Accepted(new
            {
                message = "Fotoğraf işleme alındı",
                photoId = photoRecord.Id,
                statusUrl = $"/api/photos/{photoRecord.Id}/status"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Fotoğraf yükleme hatası");
            return StatusCode(500, "Fotoğraf yüklenirken hata oluştu");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPhoto(int id)
    {
        var photo = await _context.Photos.FindAsync(id);
        if (photo == null) return NotFound();
        
        return Ok(photo);
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetPhotoStatus(int id)
    {
        var photo = await _context.Photos.FindAsync(id);
        if (photo == null) return NotFound();
        
        return Ok(new
        {
            photoId = photo.Id,
            status = photo.Status,
            errorMessage = photo.ErrorMessage,
            createdAt = photo.CreatedAt
        });
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPhotos(string userId)
    {
        var photos = await _context.Photos
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
            
        return Ok(photos);
    }
}