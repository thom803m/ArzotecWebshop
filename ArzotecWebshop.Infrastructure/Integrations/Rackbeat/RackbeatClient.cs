using System.Net.Http.Json;

namespace ArzotecWebshop.Infrastructure.Integrations.Rackbeat
{
    public class RackbeatClient
    {
        private readonly HttpClient _httpClient;

        public RackbeatClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetProductsRawAsync()
        {
            var response = await _httpClient.GetAsync("product");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
