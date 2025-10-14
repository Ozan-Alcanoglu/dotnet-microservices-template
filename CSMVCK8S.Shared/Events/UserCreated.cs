namespace CSMVCK8S.Shared.Events
{
    // Yeni bir kullanıcı oluşturulduğunda yayınlanacak olay kontratı
    public interface UserCreated
    {
        Guid UserId { get; }
        string UserName { get; }
        string Email { get; }
        DateTime CreationDate { get; }
    }
}