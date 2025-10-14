namespace UserService.Clients
{
    // Product Service'ten beklenen ürün veri transfer objesi (DTO)
    public class ProductClientDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
    }
    
    public interface IProductClient
    {
        // Tek bir ürünün detaylarını getiren metot
        Task<ProductClientDto?> GetProductDetails(int productId);
    }
}
