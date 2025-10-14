namespace UserService.Clients
{
    public interface IAuthClient
    {
        // Örnek: Bir kullanıcının geçerli olup olmadığını kontrol eden bir metot
        Task<bool> IsUserValid(string userId, string token);
    }
}