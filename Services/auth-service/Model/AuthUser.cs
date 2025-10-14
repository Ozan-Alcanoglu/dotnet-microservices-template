using CSMVCK8S.Shared.Entities;

namespace AuthService.Model
{
    public class AuthUser : BaseEntity
    {
        public string? Username { get; set; }
        public string? PasswordHash { get; set; } // Şifre Hashlenmiş olarak tutulmalı
        public string? Role { get; set; } // Örneğin "Admin", "Customer"
    }
}

