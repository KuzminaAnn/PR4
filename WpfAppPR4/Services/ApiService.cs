using Newtonsoft.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        public async Task<List<FavoriteGame>> GetFavoriteGamesAsync(int userId)
        {
            // Передаем userId прямо в адрес запроса
            var response = await _httpClient.GetAsync($"api/FavoriteGame/{userId}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FavoriteGame>>(json);
        }

        public async Task<bool> DeleteFavoriteAsync(int userId, int productId)
        {
            try
            {
                // Формируем URL: api/FavoriteGame/{userId}/{productId}
                var url = $"api/FavoriteGame/{userId}/{productId}";

                var response = await _httpClient.DeleteAsync(url);

                // Для отладки: если не 200 OK, выводим статус
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error ({response.StatusCode}): {content}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network Error: {ex.Message}");
                return false;
            }
        }
        // WpfAppPR4/Services/ApiService.cs
        public async Task<List<HistoryOrder>> GetUserOrdersAsync(int userId)
        {
            try
            {
                // Путь должен вести к новому методу, который возвращает плоский список
                var response = await _httpClient.GetAsync($"/api/history/user-orders/{userId}");

                if (!response.IsSuccessStatusCode) return new List<HistoryOrder>();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<HistoryOrder>>(content) ?? new List<HistoryOrder>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории: {ex.Message}");
                return new List<HistoryOrder>();
            }
        }
        public async Task<bool> AddToFavoritesAsync(int userId, int productId)
        {
            try
            {
                var url = $"api/FavoriteGame/{userId}/{productId}";
                var response = await _httpClient.PostAsync(url, null);

                // 200 OK = добавлено успешно
                // 409 Conflict = уже существует (это тоже нормальный исход для кнопки!)
                if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                    response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return true;
                }

                // Любая другая ошибка (500, 404 и т.д.)
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API Error: {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network Error: {ex.Message}");
                return false;
            }
        }
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/products/categories");
                if (!response.IsSuccessStatusCode) return new List<Category>();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Category>>(content) ?? new List<Category>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки категорий: {ex.Message}");
                return new List<Category>();
            }
        }
        public async Task<List<BasketItemDto>> GetUserBasketAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/basket/{userId}");

                if (!response.IsSuccessStatusCode)
                    return new List<BasketItemDto>();

                var content = await response.Content.ReadAsStringAsync();
                var basket = JsonConvert.DeserializeObject<List<BasketItemDto>>(content);

                return basket ?? new List<BasketItemDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки корзины: {ex.Message}");
                return new List<BasketItemDto>();
            }
        }
        // Удаление товара из корзины
        public async Task<bool> RemoveFromBasketAsync(int basketId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/products/basket/{basketId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка удаления: {ex.Message}");
                return false;
            }
        }

        // Изменение количества товара
        public async Task<bool> UpdateBasketCountAsync(int basketId, int newCount)
        {
            try
            {
                var content = new StringContent(newCount.ToString(), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/products/basket/{basketId}/count", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка обновления количества: {ex.Message}");
                return false;
            }
        }
        public async Task<(bool success, string error)> CreateOrderssAsync(OrderssCreateRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Отправляем запрос на создание заказа...");
                System.Diagnostics.Debug.WriteLine($"JSON: {JsonConvert.SerializeObject(request)}");

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/orderss/create", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "");
                }
                else
                {
                    return (false, $"Ошибка {response.StatusCode}: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение: {ex.Message}");
                return (false, $"Исключение: {ex.Message}");
            }
        }
        // Вставь этот метод внутрь класса ApiService

        public async Task<bool> AddToBasketAsync(int userId, int productId, decimal price)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Добавление в корзину: userId={userId}, productId={productId}, price={price}");

                var requestData = new
                {
                    IdUser = userId,
                    IdProduct = productId,
                    Count = 1,
                    SumPrice = (int)price  // Приводим к int, так как в БД SumPrice — int
                };

                var json = JsonConvert.SerializeObject(requestData);
                System.Diagnostics.Debug.WriteLine($"JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Путь должен совпадать с маршрутом в контроллере!
                var response = await _httpClient.PostAsync("/api/products/addtobasket", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response: {responseContent}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Исключение в AddToBasketAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<OrderssDto>> GetUserOrderssAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/orderss/user/{userId}");
                if (!response.IsSuccessStatusCode) return new List<OrderssDto>();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<OrderssDto>>(content) ?? new List<OrderssDto>();
            }
            catch { return new List<OrderssDto>(); }
        }

        public async Task<List<PickupPointDto>> GetPickupPointsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/orderss/pickup-points");
                if (!response.IsSuccessStatusCode) return new List<PickupPointDto>();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<PickupPointDto>>(content) ?? new List<PickupPointDto>();
            }
            catch { return new List<PickupPointDto>(); }
        }
        public async Task<List<ProductReviewDto>> GetProductReviewsAsync(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/reviews/{productId}");
                if (!response.IsSuccessStatusCode) return new List<ProductReviewDto>();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ProductReviewDto>>(content) ?? new List<ProductReviewDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки отзывов: {ex.Message}");
                return new List<ProductReviewDto>();
            }
        }
        public async Task<bool> PostReviewAsync(object reviewData)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== НАЧАЛО ОТПРАВКИ ОТЗЫВА ===");

                var json = JsonConvert.SerializeObject(reviewData);
                System.Diagnostics.Debug.WriteLine($"📤 Отправляемый JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ⚠️ ВАЖНО: Проверь, совпадает ли этот путь с атрибутом [HttpPost] в контроллере!
                string apiUrl = "/api/products/reviews";
                System.Diagnostics.Debug.WriteLine($"🌐 URL запроса: {apiUrl}");

                var response = await _httpClient.PostAsync(apiUrl, content);

                var responseBody = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"📥 Статус ответа: {(int)response.StatusCode} ({response.StatusCode})");
                System.Diagnostics.Debug.WriteLine($"📥 Тело ответа: {responseBody}");

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("✅ Отзыв успешно отправлен!");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ Сервер вернул ошибку.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 ИСКЛЮЧЕНИЕ В КЛИЕНТЕ: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }

        // Новый метод: получает все заказы и "вытаскивает" из них список товаров с ID
        public async Task<List<HistoryOrder>> GetUserOrderItemsAsync(int userId)
        {
            try
            {
                // 1. Получаем список заказов (используем уже существующий метод)
                var orders = await GetUserOrderssAsync(userId);

                if (orders == null || !orders.Any())
                    return new List<HistoryOrder>();

                // 2. Превращаем список заказов в плоский список товаров
                var historyItems = new List<HistoryOrder>();

                foreach (var order in orders)
                {
                    if (order.Items != null)
                    {
                        foreach (var item in order.Items)
                        {
                            historyItems.Add(new HistoryOrder
                            {
                                Name = item.ProductName,
                                Count = item.Count,
                                Price = item.Price,           // Цена за штуку из DTO
                                SumPrice = item.Price * item.Count, // Или item.SumPrice, если есть в DTO
                                Data = order.Data,            // Берём дату из родительского заказа
                                IdProduct = item.IdProduct    // ✅ Передаём ID товара!
                            });
                        }
                    }
                }

                // Сортируем по дате (новые сверху)
                return historyItems.OrderByDescending(h => h.Data).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки истории товаров: {ex.Message}");
                return new List<HistoryOrder>();
            }
        }
        // Метод для получения одного товара по ID (для отображения названия)
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/{productId}"); // Убедись, что такой маршрут есть в API
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Product>(content);
            }
            catch { return null; }
        }

        // Метод для отправки отзыва
        // WpfAppPR4/Services/ApiService.cs
        public async Task<bool> AddReviewAsync(int userId, int productId, string text, int rating)
        {
            try
            {
                var reviewData = new
                {
                    IdUser = userId,
                    IdProduct = productId,
                    Text = text,
                    Rating = rating
                };

                var json = JsonConvert.SerializeObject(reviewData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Путь: /api/orderss/add-review
                var response = await _httpClient.PostAsync("/api/orderss/add-review", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"ОШИБКА СЕРВЕРА: {errorContent}");
                    throw new Exception(errorContent);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}");
                return false;
            }
        }
    }
}