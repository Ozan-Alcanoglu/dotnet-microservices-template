namespace ProductService.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } // Nullable olarak işaretlendi
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}