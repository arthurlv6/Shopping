using Shared;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebBackend
{
    public class InventoryService: BaseService
    {
        private readonly HttpClient _httpClient;
        public InventoryService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task PostAsync(CartDto cartDto)
        {
            Uri url = new Uri("https://inventoryapi20201126134558.azurewebsites.net/api/stock");
            var jsonData = System.Text.Json.JsonSerializer.Serialize(cartDto);
            var modelJson =
                new StringContent(jsonData, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content=modelJson };
            var httpResponseMessage = await _httpClient.SendAsync(request);
            
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception("Failed");
            }
        }
    }
}
