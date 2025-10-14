using System.Net.Http;
using System.Threading.Tasks;

namespace UserService.Clients
{
    public class AuthClient : IAuthClient
    {
        private readonly HttpClient _httpClient;

        // IHttpClientFactory, bu HttpClient nesnesini bize enjekte eder.
        public AuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsUserValid(string userId, string token)
        {
            // HttpClient'ın BaseAddress'i Program.cs'te Auth Service'in adresi olarak ayarlanmıştır.
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/auth/validate/{userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            // Gelen cevabı işleriz (örneğin 200 OK ise true döner)
            return response.IsSuccessStatusCode;
        }
    }
}