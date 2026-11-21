using Newtonsoft.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfAppPR4.Models;


namespace WpfAppPR4.Services
{
    public class ApiService
    {
        public readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:5225";

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5225");
        }

        // WpfAppPR4/Services/ApiService.cs
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Начало загрузки продуктов...");

                // Теперь можно использовать относительный путь, так как BaseAddress задан
                var response = await _httpClient.GetAsync("/api/products/getitems");

                System.Diagnostics.Debug.WriteLine($"Статус ответа: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Ошибка сервера: {errorContent}");
                    throw new Exception($"Ошибка HTTP: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Получен JSON (первые 100 символов): {content.Substring(0, Math.Min(100, content.Length))}...");

                var products = JsonConvert.DeserializeObject<List<Product>>(content);

                if (products == null)
                {
                    System.Diagnostics.Debug.WriteLine("Десериализация вернула NULL!");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Успешно десериализовано объектов: {products.Count}");
                }

                return products ?? new List<Product>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}");
                Console.WriteLine($"Ошибка при загрузке продуктов: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<AuthResponseDto> LoginAsync(string login, string password)
        {
            try
            {
                var requestUri = $"/api/auth/login?login={Uri.EscapeDataString(login)}&password={Uri.EscapeDataString(password)}";

                var response = await _httpClient.GetAsync(requestUri);
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<AuthResponseDto>(content);

                return result ?? new AuthResponseDto { Success = false, Message = "Пустой ответ от сервера" };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto { Success = false, Message = "Ошибка: " + ex.Message };
            }
        }
        // WpfAppPR4/Services/ApiService.cs

        public async Task<List<Review>> GetUserReviewsAsync(int userId)
        {
            try
            {
                // Обрати внимание на путь: /api/r/user-reviews/... 
                // (так как контроллер называется RController и Route api/[controller])
                var response = await _httpClient.GetAsync($"/api/r/user-reviews/{userId}");

                if (!response.IsSuccessStatusCode)
                    return new List<Review>();

                var content = await response.Content.ReadAsStringAsync();

                // Теперь JsonConvert знает, что такое Review, благодаря using WpfAppPR4.Models;
                var reviews = JsonConvert.DeserializeObject<List<Review>>(content);

                return reviews ?? new List<Review>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки отзывов: {ex.Message}");
                return new List<Review>();
            }
        }
        public async Task<List<FavoriteGame>> GetFavoriteGamesAsync()
        {
            var response = await _httpClient.GetAsync("api/FavoriteGame");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FavoriteGame>>(json);
        }
    }
}