using Microsoft.EntityFrameworkCore;
using ProductService.Model;

namespace ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Statik bir tarih değeri tanımlayın (Migration için zorunlu)
            var staticDate = new DateTime(2025, 10, 10, 10, 0, 0, DateTimeKind.Utc);
            
            // Başlangıç verisi (Seed Data)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 1500.00m, Stock = 50, CreatedAt = staticDate, UpdatedAt = staticDate },
                new Product { Id = 2, Name = "Mouse", Price = 25.00m, Stock = 200, CreatedAt = staticDate, UpdatedAt = staticDate }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}