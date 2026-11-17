using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WpfAppPR4.Models;

namespace WpfAppPR4.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5225/api";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(
                    $"{BaseUrl}/products/getitems");

                var products = JsonConvert.DeserializeObject<List<Product>>(response);
                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return new List<Product>();
            }
        }
    }
}