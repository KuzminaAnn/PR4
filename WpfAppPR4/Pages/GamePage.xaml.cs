using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppPR4.Models;
using WpfAppPR4.Services;

namespace WpfAppPR4.Pages
{
    /// <summary>
    /// Логика взаимодействия для GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        private ApiService _apiService;
        private int _currentUserId = AffUser.CurrentUser.Id;
        private Product _currentProduct;
        public GamePage()
        {
            InitializeComponent();
            _apiService = new ApiService();
        }
        public async void SetProduct(Product product)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] SetProduct вызван. Product is null: {product == null}");

            if (product == null)
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] Передан null!");
                return;
            }

            _currentProduct = product;
            System.Diagnostics.Debug.WriteLine($"[OK] Товар сохранен: {product.Name}, ID: {product.Id}");
            if (product == null) return;
           
            TitleText.Text = product.Name;
            DescText.Text = product.Description;
            PriceText.Text = $"{product.Price} руб.";
            RatingText.Text = product.Rating.ToString("F1");
            GameImage.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(product.Image, UriKind.RelativeOrAbsolute));

            var reviews = await _apiService.GetProductReviewsAsync(product.Id);
            ReviewsList.ItemsSource = reviews;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartPage());
        }

        private async void Love_Click(object sender, RoutedEventArgs e)
        {
            if (_currentProduct == null || _currentProduct.Id == 0)
            {
                MessageBox.Show("Ошибка: Данные товара не загружены. Попробуйте вернуться назад и выбрать товар снова.");
                return;
            }
            if (_currentProduct == null)
            {
                MessageBox.Show("Ошибка: Товар не выбран.");
                return;
            }

            if (_currentUserId <= 0)
            {
                MessageBox.Show("Ошибка: Пользователь не авторизован.");
                return;
            }

            try
            {
                // Вызываем метод добавления в API
                bool isAdded = await _apiService.AddToFavoritesAsync(_currentUserId, _currentProduct.Id);

                if (isAdded)
                {
                    MessageBox.Show("Товар добавлен в избранное!");

                    // Опционально: меняем текст кнопки или её состояние
                    // Love.Content = "✓ В избранном";
                    // Love.IsEnabled = false; 
                }
                else
                {
                    MessageBox.Show("Не удалось добавить товар. Возможно, он уже в избранном.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сети: {ex.Message}");
            }
        }
        private void BuyNow_Click(object sender, RoutedEventArgs e)
        {
            if (_currentProduct == null) return; // Убедись, что сохранила продукт в поле _currentProduct

            var items = new List<OrderssItemRequest>
            {
            new OrderssItemRequest { IdProduct = _currentProduct.Id, Count = 1, ProductName = _currentProduct.Name, Price = (int)_currentProduct.Price }
            };

            var page = new Pages.OrderssPage(items, (int)_currentProduct.Price);
            this.NavigationService.Navigate(page);
        }
        private async void basket_Click(object sender, RoutedEventArgs e)
        {
            // Проверка авторизации
            if (AffUser.CurrentUser == null)
            {
                MessageBox.Show("Пожалуйста, войдите в аккаунт, чтобы добавлять товары в корзину!");
                return;
            }

            // Проверка товара
            if (_currentProduct == null)
            {
                MessageBox.Show("Ошибка: товар не выбран.");
                return;
            }

            // Вызов API
            bool success = await _apiService.AddToBasketAsync(
                AffUser.CurrentUser.Id,
                _currentProduct.Id,
                _currentProduct.Price
            );

            if (success)
            {
                MessageBox.Show(
                    $"Игра \"{_currentProduct.Name}\" добавлена в корзину!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else
            {
                MessageBox.Show(
                    "Не удалось добавить товар. Проверьте подключение к серверу.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}
