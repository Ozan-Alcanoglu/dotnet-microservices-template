using CSMVCK8S.Shared.Entities;

namespace UserService.Model;

public class Photo : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MinioObjectId { get; set; } = string.Empty; // MinIO'daki dosya ID'si
    public string? MinioUrl { get; set; } // MinIO'daki dosya URL'si (opsiyonel)
    public long FileSize { get; set; }
    public string Status { get; set; } = "processing"; // "processing", "completed", "failed"
    public string? ErrorMessage { get; set; }
}

// DbContext'te sadece DbSet ekle:
