using CSMVCK8S.Shared.Entities;

namespace ProductService.Model
{
    public class Product : BaseEntity
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}