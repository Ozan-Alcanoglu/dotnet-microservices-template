using MassTransit;

namespace CSMVCK8S.Shared.Events;

public class PhotoUploadEvent
{
    public int PhotoId { get; set; }                    
    public string MinioObjectId { get; set; } = string.Empty; 
    public string FileName { get; set; } = string.Empty;
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public string UserId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

public class PhotoReadyForUploadEvent
{
    public int PhotoId { get; set; }
    public string MinioObjectId { get; set; } = string.Empty;
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}
public class PhotoUploadCompletedEvent
{
    public int PhotoId { get; set; }
    public string Status { get; set; } = "completed";
    public string? MinioUrl { get; set; }
    public string? ErrorMessage { get; set; }
}