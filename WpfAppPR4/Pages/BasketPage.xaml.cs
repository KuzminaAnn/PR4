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
    /// Логика взаимодействия для BasketPage.xaml
    /// </summary>
    public partial class BasketPage : Page
    {
        private readonly ApiService _apiService;
        private List<BasketItemDto> _basketItems = new List<BasketItemDto>();
        public BasketPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Loaded += async (s, e) => await LoadBasket();
        }
        private async Task LoadBasket()
        {
            if (AffUser.CurrentUser == null)
            {
                MessageBox.Show("Вы не авторизованы!");
                return;
            }

            int userId = AffUser.CurrentUser.Id;
            _basketItems = await _apiService.GetUserBasketAsync(userId);

            BasketListBox.ItemsSource = _basketItems;
            UpdateTotalSum();
        }

        private void UpdateTotalSum()
        {
            decimal totalSum = _basketItems.Sum(item => item.SumPrice);
            TotalSumText.Text = $"Итого: {totalSum:C}";
        }

        // Кнопка "+"
        private async void IncreaseCount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int basketId)
            {
                var item = _basketItems.FirstOrDefault(b => b.Id == basketId);
                if (item != null)
                {
                    int newCount = item.Count + 1;
                    bool success = await _apiService.UpdateBasketCountAsync(basketId, newCount);

                    if (success)
                    {
                        await LoadBasket(); // Перезагружаем корзину
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить количество");
                    }
                }
            }
        }
        private async void DecreaseCount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int basketId)
            {
                var item = _basketItems.FirstOrDefault(b => b.Id == basketId);
                if (item != null && item.Count > 1)
                {
                    int newCount = item.Count - 1;
                    bool success = await _apiService.UpdateBasketCountAsync(basketId, newCount);

                    if (success)
                    {
                        await LoadBasket();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить количество");
                    }
                }
            }
        }
        private async void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int basketId)
            {
                var result = MessageBox.Show("Удалить товар из корзины?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = await _apiService.RemoveFromBasketAsync(basketId);

                    if (success)
                    {
                        await LoadBasket();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить товар");
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartPage());
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            if (_basketItems == null || !_basketItems.Any()) { MessageBox.Show("Корзина пуста!"); return; }

            var items = _basketItems.Select(b => new OrderssItemRequest
            {
                IdProduct = b.IdProduct,
                Count = b.Count,
                ProductName = b.ProductName,
                Price = (int)b.SumPrice // или b.Price * b.Count, в зависимости от того, что хранится
            }).ToList();

            int total = _basketItems.Sum(b => (int)b.SumPrice);

            var page = new Pages.OrderssPage(items, total);
            this.NavigationService.Navigate(page);
        }
    }
}
