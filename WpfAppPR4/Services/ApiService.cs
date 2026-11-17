using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WpfAppPR4.Models;

namespace WpfAppPR4.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5225"),
                Timeout = TimeSpan.FromSeconds(10)
            };
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
    }
}