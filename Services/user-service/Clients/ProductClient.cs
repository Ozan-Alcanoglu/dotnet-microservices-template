using System.Net.Http.Json;
using System.Threading.Tasks;

namespace UserService.Clients
{
    public class ProductClient : IProductClient
    {
        private readonly HttpClient _httpClient;

        // IHttpClientFactory tarafından enjekte edilen HttpClient
        public ProductClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductClientDto?> GetProductDetails(int productId)
        {
            // Product Service'in API'sine istek atma
            // BaseAddress, Program.cs'te ayarlandı
            var response = await _httpClient.GetAsync($"/api/Product/{productId}");

            if (response.IsSuccessStatusCode)
            {
                // JSON cevabını ProductClientDto'ya dönüştür
                return await response.Content.ReadFromJsonAsync<ProductClientDto>();
            }

            // Hata durumunda null dön
            return null;
        }
    }
}
