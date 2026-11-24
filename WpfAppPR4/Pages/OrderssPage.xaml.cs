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
    /// Логика взаимодействия для OrderssPage.xaml
    /// </summary>
    public partial class OrderssPage : Page
    {
        private readonly ApiService _apiService;
        private List<OrderssItemRequest> _orderItems;
        private int _totalPrice;
        public OrderssPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            Loaded += OrderssPage_Loaded;
        }
        // Конструктор для приема товаров
        public OrderssPage(List<OrderssItemRequest> items, int totalPrice) : this()
        {
            _orderItems = items;
            _totalPrice = totalPrice;
        }

        private async void OrderssPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_orderItems == null || !_orderItems.Any())
            {
                MessageBox.Show("Нет товаров для заказа!");
                return;
            }

            OrderItemsList.ItemsSource = _orderItems;
            TotalText.Text = $"Итого: {_totalPrice} руб.";

            // Логика ограничения доставки (укажи правильные ID из таблицы DeliveryType)
            // Допустим, 1 = Доставка, 2 = ПВЗ. Проверь свою БД!
            int deliveryId = 1;
            if (_totalPrice < 600)
            {
                RbDelivery.IsEnabled = false;
                RbDelivery.Content = "Доставка недоступна (заказ < 600 руб.)";
                RbPickup.IsChecked = true;
            }
            else
            {
                RbPickup.IsChecked = true;
            }

            var points = await _apiService.GetPickupPointsAsync();
            PickupCombo.ItemsSource = points;
            if (points.Any()) PickupCombo.SelectedIndex = 0;

            DeliveryType_Changed(null, null);
        }

        private void DeliveryType_Changed(object sender, RoutedEventArgs e)
        {
            if (RbPickup.IsChecked == true)
            {
                PickupPanel.Visibility = Visibility.Visible;
                AddressPanel.Visibility = Visibility.Collapsed;
            }
            else if (RbDelivery.IsChecked == true)
            {
                PickupPanel.Visibility = Visibility.Collapsed;
                AddressPanel.Visibility = Visibility.Visible;
            }
        }

        private async void SubmitOrderss_Click(object sender, RoutedEventArgs e)
        {
            if (AffUser.CurrentUser == null) { MessageBox.Show("Вы не авторизованы!"); return; }

            int deliveryTypeId = RbPickup.IsChecked == true ? 2 : 1; // Проверь свои ID!

            string address = deliveryTypeId == 1 ? AddressBox.Text?.Trim() : "";
            int? pickupId = deliveryTypeId == 2 ? (PickupCombo.SelectedItem as PickupPointDto)?.Id : null;

            if (deliveryTypeId == 1 && string.IsNullOrEmpty(address)) { MessageBox.Show("Укажите адрес!"); return; }
            if (deliveryTypeId == 2 && pickupId == null) { MessageBox.Show("Выберите ПВЗ!"); return; }

            var request = new OrderssCreateRequest
            {
                IdUser = AffUser.CurrentUser.Id,
                DeliveryTypeId = deliveryTypeId,
                Address = address,
                PickupPointId = pickupId,
                TotalPrice = _totalPrice,
                Items = _orderItems
            };

            var (success, error) = await _apiService.CreateOrderssAsync(request);

            if (success)
            {
                MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService?.Navigate(new StartPage());
            }
            else
            {
                // Теперь мы видим РЕАЛЬНУЮ ошибку
                MessageBox.Show($"Не удалось оформить заказ.\n\nОшибка: {error}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
