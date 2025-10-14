using Microsoft.EntityFrameworkCore;
using UserService.Model;
using CSMVCK8S.Shared.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Statik bir tarih değeri tanımlayın
            var staticDate = new DateTime(2025, 10, 10, 10, 0, 0, DateTimeKind.Utc);
            
            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id = 1, 
                    Email = "test1@mail.com", 
                    FirstName = "Ahmet", 
                    LastName = "Yılmaz",
                    CreatedAt = staticDate, // << Statik değer atandı
                    UpdatedAt = staticDate  // << Statik değer atandı
                },
                new User { 
                    Id = 2, 
                    Email = "test2@mail.com", 
                    FirstName = "Ayşe", 
                    LastName = "Kaya",
                    CreatedAt = staticDate, // << Statik değer atandı
                    UpdatedAt = staticDate  // << Statik değer atandı
                }
            );

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasIndex(p => p.UserId);
                entity.HasIndex(p => p.MinioObjectId).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}