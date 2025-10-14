using Microsoft.EntityFrameworkCore;
using AuthService.Model;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<AuthUser> AuthUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var staticDate = new DateTime(2025, 10, 10, 10, 0, 0, DateTimeKind.Utc);
            
            // Başlangıç verisi: BASIT ŞİFRE KONTROLÜ İÇİN
            modelBuilder.Entity<AuthUser>().HasData(
                new AuthUser { 
                    Id = 1, 
                    Username = "admin", 
                    PasswordHash = "123456", // Güvenlik için hash'lenmeli!
                    Role = "Admin",
                    CreatedAt = staticDate, 
                    UpdatedAt = staticDate 
                }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}