namespace AuthService.DTO
{
    public class AuthTokenDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; } // Genellikle kullanılır
        public DateTime ExpiresIn { get; set; }
    }
}