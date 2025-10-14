using System.ComponentModel.DataAnnotations;

namespace ProductService.DTO
{
    public class ProductCreateDto
    {
        [Required]
        public string? Name { get; set; }
        
        [Range(0.01, 100000.00)]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}