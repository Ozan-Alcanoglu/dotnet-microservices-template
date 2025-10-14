namespace CSMVCK8S.Shared.Entities
{
    // Veritabanı Entity'lerinin temel özelliklerini tanımlar.
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Varsayılan değer

        // Yeni oluşturulacak nesneler için tarih ayarı
        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}