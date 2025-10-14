using AuthService.Data;
using AuthService.DTO;
using AuthService.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AuthDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthTokenDto?> LoginAsync(LoginDto loginDto)
        {
            // Gerçek projede: PasswordHash ile şifre doğrulaması yapılmalıdır.
            var user = new AuthUser
            {
                Id = 1,
                Username = "testuser",
                PasswordHash = "password", // Basit örnek için düz metin
                Role = "User"
            };


            var token = GenerateJwtToken(user);
            
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");
            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            return new AuthTokenDto
            {
                AccessToken = token,
                ExpiresIn = expires,
                RefreshToken = Guid.NewGuid().ToString() // Basit bir Refresh Token
            };
        }

        public string GenerateJwtToken(AuthUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            
            // ✅ NULL KONTROLLERİ
            var secret = jwtSettings["Secret"] ?? throw new Exception("JWT Secret is missing!");
            var issuer = jwtSettings["Issuer"] ?? "CSMVCK8S.AuthService";
            var audience = jwtSettings["Audience"] ?? "CSMVCK8S.Microservices";
            var expiryMinutes = jwtSettings.GetValue<int>("ExpiryMinutes", 60);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role!)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}