using AuthService.DTO;
using AuthService.Model;
using System.Security.Claims;

namespace AuthService.Services
{
    public interface IAuthService
    {
        Task<AuthTokenDto?> LoginAsync(LoginDto loginDto);
        string GenerateJwtToken(AuthUser user);
    }
}